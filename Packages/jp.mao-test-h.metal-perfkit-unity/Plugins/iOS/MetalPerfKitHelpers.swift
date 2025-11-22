import UIKit
import Metal

// MARK: - Status Codes
public enum MPHStatus: Int32 {
    case success = 1
    case failure = 0
    case error = -1
}

// MARK: - Helper Functions

func getMetalLayer() -> CAMetalLayer? {
    guard let instance = UnityFramework.getInstance(),
          let rootView = instance.appController().rootView,
          let metalLayer = rootView.layer as? CAMetalLayer else {
        return nil
    }

    return metalLayer
}

func getCurrentProperties() -> [String: Any] {
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

func updateProperties(_ updates: [String: Any]) {
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

func removeProperties(_ keys: [String]) {
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

func getReferenceSize() -> CGSize {
    guard let metalLayer = getMetalLayer() else {
        preconditionFailure()
    }

    return metalLayer.bounds.size
}

func normalizedToAbsolute(x normalizedX: Float, y normalizedY: Float) -> (x: Float, y: Float) {
    let referenceSize = getReferenceSize()
    let absoluteX = normalizedX * Float(referenceSize.width)
    let absoluteY = normalizedY * Float(referenceSize.height)
    return (x: absoluteX, y: absoluteY)
}

func absoluteToNormalized(x absoluteX: Float, y absoluteY: Float) -> (x: Float, y: Float) {
    let referenceSize = getReferenceSize()
    let normalizedX = absoluteX / Float(referenceSize.width)
    let normalizedY = absoluteY / Float(referenceSize.height)
    return (x: normalizedX, y: normalizedY)
}

func metricElementsToString(_ elements: UInt32) -> String {
    var metrics: [String] = []

    if elements & (1 << 0) != 0 { metrics.append("device") }
    if elements & (1 << 1) != 0 { metrics.append("rosetta") }
    if elements & (1 << 2) != 0 { metrics.append("layersize") }
    if elements & (1 << 3) != 0 { metrics.append("layerscale") }
    if elements & (1 << 4) != 0 { metrics.append("memory") }
    if elements & (1 << 5) != 0 { metrics.append("fps") }
    if elements & (1 << 6) != 0 { metrics.append("frameinterval") }
    if elements & (1 << 7) != 0 { metrics.append("gputime") }
    if elements & (1 << 8) != 0 { metrics.append("thermal") }
    if elements & (1 << 9) != 0 { metrics.append("frameintervalgraph") }
    if elements & (1 << 10) != 0 { metrics.append("presentdelay") }
    if elements & (1 << 11) != 0 { metrics.append("frameintervalhistogram") }
    if elements & (1 << 12) != 0 { metrics.append("metalcpu") }
    if elements & (1 << 13) != 0 { metrics.append("gputimeline") }
    if elements & (1 << 14) != 0 { metrics.append("shaders") }
    if elements & (1 << 15) != 0 { metrics.append("framenumber") }
    if elements & (1 << 16) != 0 { metrics.append("disk") }
    if elements & (1 << 17) != 0 { metrics.append("fpsgraph") }
    if elements & (1 << 18) != 0 { metrics.append("toplabeledcommandbuffers") }
    if elements & (1 << 19) != 0 { metrics.append("toplabeledencoders") }

    return metrics.joined(separator: ",")
}

func stringToMetricElements(_ elementsString: String) -> UInt32 {
    var elements: UInt32 = 0
    let metricsArray = elementsString.split(separator: ",").map { $0.trimmingCharacters(in: .whitespaces) }

    for metric in metricsArray {
        switch metric {
        case "device": elements |= (1 << 0)
        case "rosetta": elements |= (1 << 1)
        case "layersize": elements |= (1 << 2)
        case "layerscale": elements |= (1 << 3)
        case "memory": elements |= (1 << 4)
        case "fps": elements |= (1 << 5)
        case "frameinterval": elements |= (1 << 6)
        case "gputime": elements |= (1 << 7)
        case "thermal": elements |= (1 << 8)
        case "frameintervalgraph": elements |= (1 << 9)
        case "presentdelay": elements |= (1 << 10)
        case "frameintervalhistogram": elements |= (1 << 11)
        case "metalcpu": elements |= (1 << 12)
        case "gputimeline": elements |= (1 << 13)
        case "shaders": elements |= (1 << 14)
        case "framenumber": elements |= (1 << 15)
        case "disk": elements |= (1 << 16)
        case "fpsgraph": elements |= (1 << 17)
        case "toplabeledcommandbuffers": elements |= (1 << 18)
        case "toplabeledencoders": elements |= (1 << 19)
        default: break
        }
    }

    return elements
}

func alignmentToString(_ alignment: Int32) -> String? {
    switch alignment {
    case 0: return "topleft"
    case 1: return "topcenter"
    case 2: return "topright"
    case 3: return "centerleft"
    case 4: return "centered"
    case 5: return "centerright"
    case 6: return "bottomleft"
    case 7: return "bottomcenter"
    case 8: return "bottomright"
    default: return nil
    }
}

func stringToAlignment(_ alignmentString: String) -> Int32 {
    switch alignmentString {
    case "topleft": return 0
    case "topcenter": return 1
    case "topright": return 2
    case "centerleft": return 3
    case "centered": return 4
    case "centerright": return 5
    case "bottomleft": return 6
    case "bottomcenter": return 7
    case "bottomright": return 8
    default: return -1
    }
}
