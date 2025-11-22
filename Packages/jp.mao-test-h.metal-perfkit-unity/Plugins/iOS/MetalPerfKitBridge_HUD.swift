import UIKit

// MARK: - HUD Control APIs

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

@_cdecl("MetalPerfKit_GetOpacity")
public func MetalPerfKit_GetOpacity(_ outValue: UnsafeMutablePointer<Float>?) -> Int32 {
    guard let _ = getMetalLayer(), let outValue else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let opacity = properties["MTL_HUD_OPACITY"] as? Float {
        outValue.pointee = opacity
    } else {
        outValue.pointee = 1.0  // default value
    }

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_GetScale")
public func MetalPerfKit_GetScale(_ outValue: UnsafeMutablePointer<Float>?) -> Int32 {
    guard let _ = getMetalLayer(), let outValue else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let scale = properties["MTL_HUD_SCALE"] as? Float {
        outValue.pointee = scale
    } else {
        outValue.pointee = 0.2  // default value
    }

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_GetAlignment")
public func MetalPerfKit_GetAlignment(_ outValue: UnsafeMutablePointer<Int32>?) -> Int32 {
    guard let _ = getMetalLayer(), let outValue else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let alignment = properties["MTL_HUD_ALIGNMENT"] as? String {
        outValue.pointee = stringToAlignment(alignment)
    } else {
        outValue.pointee = 2  // default: topright
    }

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_GetElements")
public func MetalPerfKit_GetElements(_ outValue: UnsafeMutablePointer<UInt32>?) -> Int32 {
    guard let _ = getMetalLayer(), let outValue else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let elements = properties["MTL_HUD_ELEMENTS"] as? String {
        outValue.pointee = stringToMetricElements(elements)
    } else {
        outValue.pointee = 0  // default: none
    }

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_GetShowZeroMetrics")
public func MetalPerfKit_GetShowZeroMetrics() -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let value = properties["MTL_HUD_SHOW_ZERO_METRICS"] as? String {
        let status = value == "1" ? MPHStatus.success : MPHStatus.failure
        return status.rawValue
    }

    return MPHStatus.failure.rawValue
}

@_cdecl("MetalPerfKit_GetShowMetricsRange")
public func MetalPerfKit_GetShowMetricsRange() -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let value = properties["MTL_HUD_SHOW_METRICS_RANGE"] as? String {
        let status = value == "1" ? MPHStatus.success : MPHStatus.failure
        return status.rawValue
    }

    return MPHStatus.failure.rawValue
}

@_cdecl("MetalPerfKit_GetMetricTimeout")
public func MetalPerfKit_GetMetricTimeout(_ outValue: UnsafeMutablePointer<Int32>?) -> Int32 {
    guard let _ = getMetalLayer(), let outValue else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let timeout = properties["MTL_HUD_METRIC_TIMEOUT"] as? Int32 {
        outValue.pointee = timeout
    } else {
        outValue.pointee = 5  // default value
    }

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_GetEncoderTimingEnabled")
public func MetalPerfKit_GetEncoderTimingEnabled() -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let value = properties["MTL_HUD_ENCODER_TIMING_ENABLED"] as? String {
        let status = value == "1" ? MPHStatus.success : MPHStatus.failure
        return status.rawValue
    }

    return MPHStatus.failure.rawValue
}

@_cdecl("MetalPerfKit_GetEncoderGpuTimelineFrameCount")
public func MetalPerfKit_GetEncoderGpuTimelineFrameCount(_ outValue: UnsafeMutablePointer<Int32>?) -> Int32 {
    guard let _ = getMetalLayer(), let outValue else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let count = properties["MTL_HUD_ENCODER_GPU_TIMELINE_FRAME_COUNT"] as? Int32 {
        outValue.pointee = count
    } else {
        outValue.pointee = 6  // default value
    }

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_GetEncoderGpuTimelineSwapDelta")
public func MetalPerfKit_GetEncoderGpuTimelineSwapDelta(_ outValue: UnsafeMutablePointer<Int32>?) -> Int32 {
    guard let _ = getMetalLayer(), let outValue else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let delta = properties["MTL_HUD_ENCODER_GPU_TIMELINE_SWAP_DELTA"] as? Int32 {
        outValue.pointee = delta
    } else {
        outValue.pointee = 1  // default value
    }

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_GetRusageUpdateInterval")
public func MetalPerfKit_GetRusageUpdateInterval(_ outValue: UnsafeMutablePointer<Int32>?) -> Int32 {
    guard let _ = getMetalLayer(), let outValue else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let interval = properties["MTL_HUD_RUSAGE_UPDATE_INTERVAL"] as? Int32 {
        outValue.pointee = interval
    } else {
        outValue.pointee = 3  // default value
    }

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_GetDisableMenuBar")
public func MetalPerfKit_GetDisableMenuBar() -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let value = properties["MTL_HUD_DISABLE_MENU_BAR"] as? String {
        let status = value == "1" ? MPHStatus.success : MPHStatus.failure
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

@_cdecl("MetalPerfKit_SetOpacity")
public func MetalPerfKit_SetOpacity(_ value: Float) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    updateProperties(["MTL_HUD_OPACITY": value])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetScale")
public func MetalPerfKit_SetScale(_ value: Float) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    updateProperties(["MTL_HUD_SCALE": value])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetAlignment")
public func MetalPerfKit_SetAlignment(_ alignment: Int32) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    guard let alignmentString = alignmentToString(alignment) else {
        return MPHStatus.failure.rawValue
    }

    updateProperties(["MTL_HUD_ALIGNMENT": alignmentString])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetElements")
public func MetalPerfKit_SetElements(_ elements: UInt32) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    let elementsString = metricElementsToString(elements)
    updateProperties(["MTL_HUD_ELEMENTS": elementsString])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetShowZeroMetrics")
public func MetalPerfKit_SetShowZeroMetrics(_ enabled: UInt8) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    let value = enabled == 1 ? "1" : "0"
    updateProperties(["MTL_HUD_SHOW_ZERO_METRICS": value])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetShowMetricsRange")
public func MetalPerfKit_SetShowMetricsRange(_ enabled: UInt8) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    let value = enabled == 1 ? "1" : "0"
    updateProperties(["MTL_HUD_SHOW_METRICS_RANGE": value])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetMetricTimeout")
public func MetalPerfKit_SetMetricTimeout(_ seconds: Int32) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    updateProperties(["MTL_HUD_METRIC_TIMEOUT": seconds])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetEncoderTimingEnabled")
public func MetalPerfKit_SetEncoderTimingEnabled(_ enabled: UInt8) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    let value = enabled == 1 ? "1" : "0"
    updateProperties(["MTL_HUD_ENCODER_TIMING_ENABLED": value])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetEncoderGpuTimelineFrameCount")
public func MetalPerfKit_SetEncoderGpuTimelineFrameCount(_ count: Int32) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    updateProperties(["MTL_HUD_ENCODER_GPU_TIMELINE_FRAME_COUNT": count])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetEncoderGpuTimelineSwapDelta")
public func MetalPerfKit_SetEncoderGpuTimelineSwapDelta(_ seconds: Int32) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    updateProperties(["MTL_HUD_ENCODER_GPU_TIMELINE_SWAP_DELTA": seconds])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetRusageUpdateInterval")
public func MetalPerfKit_SetRusageUpdateInterval(_ seconds: Int32) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    updateProperties(["MTL_HUD_RUSAGE_UPDATE_INTERVAL": seconds])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetDisableMenuBar")
public func MetalPerfKit_SetDisableMenuBar(_ disable: UInt8) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    let value = disable == 1 ? "1" : "0"
    updateProperties(["MTL_HUD_DISABLE_MENU_BAR": value])

    return MPHStatus.success.rawValue
}
