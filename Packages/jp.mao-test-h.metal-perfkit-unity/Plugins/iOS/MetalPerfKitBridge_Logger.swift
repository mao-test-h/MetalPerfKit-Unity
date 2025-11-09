import UIKit
import OSLog

// MARK: - Logging Control APIs

// MARK: - Get API

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

@_cdecl("MetalPerfKit_GetShaderLoggingEnabled")
public func MetalPerfKit_GetShaderLoggingEnabled() -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let value = properties["MTL_HUD_LOG_SHADER_ENABLED"] as? String {
        let status = value == "1" ? MPHStatus.success : MPHStatus.failure
        return status.rawValue
    }

    return MPHStatus.failure.rawValue
}

// MARK: - Set API

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

@_cdecl("MetalPerfKit_SetShaderLoggingEnabled")
public func MetalPerfKit_SetShaderLoggingEnabled(_ enabled: UInt8) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    let value = enabled == 1 ? "1" : "0"
    updateProperties(["MTL_HUD_LOG_SHADER_ENABLED": value])

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
