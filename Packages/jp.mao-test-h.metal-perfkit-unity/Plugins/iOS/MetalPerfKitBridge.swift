import UIKit
import Metal
import OSLog

// MARK: - Status Codes
public enum MPHStatus: Int32 {
    case success = 1
    case failure = 0
    case error = -1
}

// MARK: - Helper Functions

private func getMetalLayer() -> CAMetalLayer? {
    guard let instance = UnityFramework.getInstance(),
          let rootView = instance.appController().rootView,
          let metalLayer = rootView.layer as? CAMetalLayer else {
        return nil
    }
    
    return metalLayer
}

private func getCurrentProperties() -> [String: Any] {
    guard #available(iOS 16.0, *) else {
        print("On iOS < 16  Metal Performance HUD is unavailable.")
        return [:]
    }
    
    guard let metalLayer = getMetalLayer(),
          let currentProperties = metalLayer.developerHUDProperties else {
        // NOTE: 設定アプリやスキーム側の設定問わず、初期状態は確実に nil が入っているっぽいので、一旦は空の状態で渡すようにする
        return [:]
    }
    
    return currentProperties as! [String : Any]
}

private func updateProperties(_ updates: [String: Any]) {
    guard #available(iOS 16.0, *) else {
        print("On iOS < 16  Metal Performance HUD is unavailable.")
        return
    }
    
    guard let metalLayer = getMetalLayer() else {
        return
    }
    
    var currentProperties = getCurrentProperties()
    for (key, value) in updates {
        currentProperties[key] = value
    }
    
    print("update properties: \(currentProperties)")
    metalLayer.developerHUDProperties = currentProperties
}

private func removeProperties(_ keys: [String]) {
    guard #available(iOS 16.0, *) else {
        print("On iOS < 16  Metal Performance HUD is unavailable.")
        return
    }
    
    guard let metalLayer = getMetalLayer() else {
        return
    }
    
    var currentProperties = getCurrentProperties()
    for key in keys {
        currentProperties.removeValue(forKey: key)
    }
    
    print("removed properties: \(currentProperties)")
    metalLayer.developerHUDProperties = currentProperties
}

private func getReferenceSize() -> CGSize {
    guard let metalLayer = getMetalLayer() else {
        preconditionFailure()
    }
    
    return metalLayer.bounds.size
}

private func normalizedToAbsolute(x normalizedX: Float, y normalizedY: Float) -> (x: Float, y: Float) {
    let referenceSize = getReferenceSize()
    let absoluteX = normalizedX * Float(referenceSize.width)
    let absoluteY = normalizedY * Float(referenceSize.height)
    return (x: absoluteX, y: absoluteY)
}

private func absoluteToNormalized(x absoluteX: Float, y absoluteY: Float) -> (x: Float, y: Float) {
    let referenceSize = getReferenceSize()
    let normalizedX = absoluteX / Float(referenceSize.width)
    let normalizedY = absoluteY / Float(referenceSize.height)
    return (x: normalizedX, y: normalizedY)
}

// MARK: - Get API

@_cdecl("MetalPerfKit_GetHUDVisible")
public func MetalPerfKit_GetHUDVisible() -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }
    
    let properties = getCurrentProperties()
    if let mode = properties["mode"] as? String {
        let status = (mode == "default" || mode == "main") ? MPHStatus.success : MPHStatus.failure
        return status.rawValue
    }
    
    return MPHStatus.failure.rawValue
}


@_cdecl("MetalPerfKit_GetHUDPosition")
public func MetalPerfKit_GetHUDPosition(_ outNormalizedX: UnsafeMutablePointer<Float>?, _ outNormalizedY: UnsafeMutablePointer<Float>?) -> Int32 {
    guard let _ = getMetalLayer(), let outNormalizedX, let outNormalizedY else {
        return MPHStatus.error.rawValue
    }
    
    let properties = getCurrentProperties()
    if #available(iOS 26.0, *) {
        if let absoluteX = properties["positionX"] as? Float,
           let absoluteY = properties["positionY"] as? Float {
            let normalized = absoluteToNormalized(x: absoluteX, y: absoluteY)
            outNormalizedX.pointee = normalized.x
            outNormalizedY.pointee = normalized.y
        } else {
            outNormalizedX.pointee = 0.0
            outNormalizedY.pointee = 0.0
        }
    } else {
        outNormalizedX.pointee = 0.0
        outNormalizedY.pointee = 0.0
    }
    
    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_EnabledLogging")
public func MetalPerfKit_EnabledLogging() -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }
    
    let properties = getCurrentProperties()
    if let logging = properties["logging"] as? String {
        let status = logging == "default" ? MPHStatus.success : MPHStatus.failure
        return status.rawValue
    }
    
    return MPHStatus.failure.rawValue
}

// MARK: - Set API

@_cdecl("MetalPerfKit_SetHUDVisible")
public func MetalPerfKit_SetHUDVisible(_ visible: UInt8) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }
    
    let visible = visible == 1
    if #available(iOS 26.0, *) {
        // NOTE: 一度 position を設定してしまうと、その後プロパティから削除しても位置が反映され続けるので注意
        removeProperties(["positionX", "positionY"])
        updateProperties(["mode": visible ? "default" : "disabled"])
    } else {
        if visible {
            updateProperties(["mode": "default"])
        } else {
            removeProperties(["mode"])
        }
    }
    
    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetHUDVisibleWithPosition")
public func MetalPerfKit_SetHUDVisibleWithPosition(_ visible: UInt8, _ normalizedX: Float, _ normalizedY: Float) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }
    
    let visible = visible == 1
    if #available(iOS 26.0, *) {
        var updates: [String: Any] = ["mode": visible ? "default" : "disabled"]
        let absolute = normalizedToAbsolute(x: normalizedX, y: normalizedY)
        updates["positionX"] = absolute.x
        updates["positionY"] = absolute.y
        updateProperties(updates)
    } else {
        if visible {
            updateProperties(["mode": "default"])
        } else {
            removeProperties(["mode"])
        }
    }
    
    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetLogging")
public func MetalPerfKit_SetLogging(_ enabled: UInt8) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }
    
    let enabled = enabled == 1
    if #available(iOS 26.0, *) {
        updateProperties(["logging": enabled ? "default" : "disabled"])
    } else {
        if enabled {
            updateProperties(["logging": "default"])
        } else {
            removeProperties(["logging"])
        }
    }
    return MPHStatus.success.rawValue
}

// MARK: - Log Fetching API

@_cdecl("MetalPerfKit_FetchLogs")
public func MetalPerfKit_FetchLogs(_ pastSeconds: Int32, _ savePath: UnsafePointer<CChar>?) -> Int32 {
    guard let savePath = savePath else {
        print("[MetalPerfKit] Error: savePath is nil")
        return MPHStatus.error.rawValue
    }
    
    let savePathString = String(cString: savePath)
    
    if #available(iOS 15.0, *) {
        do {
            let logStore = try OSLogStore(scope: .currentProcessIdentifier)
            
            let date = Date()
            let pastDate = date.addingTimeInterval(-TimeInterval(pastSeconds))
            let position = logStore.position(date: date)
            
            // "metal-HUD:"で始まるログをフィルタリング
            let predicate = NSPredicate(format: "composedMessage BEGINSWITH 'metal-HUD:'")
            let entries = try logStore.getEntries(with: .reverse, at: position, matching: predicate)
                .filter { entry in
                    return entry.date >= pastDate && entry.date <= date
                }
            
            // NOTE: フレーム番号が重複するケースがあるので取り除く
            var seenFrameNumbers = Set<String>()
            var logLines: [String] = []
            for entry in entries {
                let message = entry.composedMessage
                // フレーム番号を抽出（最初のカンマまでの数値）
                let afterPrefix = message.dropFirst("metal-HUD:".count).trimmingCharacters(in: .whitespaces)
                if let firstCommaIndex = afterPrefix.firstIndex(of: ",") {
                    let frameNumber = String(afterPrefix[..<firstCommaIndex]).trimmingCharacters(in: .whitespaces)
                    
                    // 重複チェック
                    if !seenFrameNumbers.contains(frameNumber) {
                        seenFrameNumbers.insert(frameNumber)
                        //logLines.append("\(entry.date): \(message)")
                        logLines.append(message)
                    }
                }
            }
            
            let logContent = logLines.joined(separator: "\n")
            let fileURL = URL(fileURLWithPath: savePathString)
            try logContent.write(to: fileURL, atomically: true, encoding: .utf8)
            
            print("[MetalPerfKit] Successfully fetched \(logLines.count) log entries to \(savePathString)")
            return MPHStatus.success.rawValue
            
        } catch {
            print("[MetalPerfKit] Error fetching logs: \(error)")
            return MPHStatus.failure.rawValue
        }
    } else {
        print("[MetalPerfKit] Error: OSLogStore requires iOS 15.0 or later")
        return MPHStatus.failure.rawValue
    }
}
