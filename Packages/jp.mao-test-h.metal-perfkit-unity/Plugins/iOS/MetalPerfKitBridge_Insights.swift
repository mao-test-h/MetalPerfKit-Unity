import UIKit

// MARK: - Performance Insights & Report APIs

// MARK: - Get API

@_cdecl("MetalPerfKit_GetInsightsEnabled")
public func MetalPerfKit_GetInsightsEnabled() -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let value = properties["MTL_HUD_INSIGHTS_ENABLED"] as? String {
        let status = value == "1" ? MPHStatus.success : MPHStatus.failure
        return status.rawValue
    }

    return MPHStatus.failure.rawValue
}

@_cdecl("MetalPerfKit_GetInsightTimeout")
public func MetalPerfKit_GetInsightTimeout(_ outValue: UnsafeMutablePointer<Int32>?) -> Int32 {
    guard let _ = getMetalLayer(), let outValue else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let timeout = properties["MTL_HUD_INSIGHT_TIMEOUT"] as? Int32 {
        outValue.pointee = timeout
    } else {
        outValue.pointee = 10  // default value
    }

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_GetInsightReportInterval")
public func MetalPerfKit_GetInsightReportInterval(_ outValue: UnsafeMutablePointer<Int32>?) -> Int32 {
    guard let _ = getMetalLayer(), let outValue else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let interval = properties["MTL_HUD_INSIGHT_REPORT_INTERVAL"] as? Int32 {
        outValue.pointee = interval
    } else {
        outValue.pointee = 5  // default value
    }

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_GetReportPath")
public func MetalPerfKit_GetReportPath(_ outPath: UnsafeMutablePointer<CChar>?, _ maxLength: Int32) -> Int32 {
    guard let _ = getMetalLayer(), let outPath else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let path = properties["MTL_HUD_REPORT_URL"] as? String {
        let cString = Array(path.utf8CString)
        let copyLength = min(cString.count, Int(maxLength))
        for i in 0..<copyLength {
            outPath[i] = CChar(cString[i])
        }
    } else {
        outPath[0] = 0  // empty string
    }

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_GetConfigFilePath")
public func MetalPerfKit_GetConfigFilePath(_ outPath: UnsafeMutablePointer<CChar>?, _ maxLength: Int32) -> Int32 {
    guard let _ = getMetalLayer(), let outPath else {
        return MPHStatus.error.rawValue
    }

    let properties = getCurrentProperties()
    if let path = properties["MTL_HUD_CONFIG_FILE"] as? String {
        let cString = Array(path.utf8CString)
        let copyLength = min(cString.count, Int(maxLength))
        for i in 0..<copyLength {
            outPath[i] = CChar(cString[i])
        }
    } else {
        outPath[0] = 0  // empty string
    }

    return MPHStatus.success.rawValue
}

// MARK: - Set API

@_cdecl("MetalPerfKit_SetInsightsEnabled")
public func MetalPerfKit_SetInsightsEnabled(_ enabled: UInt8) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    let value = enabled == 1 ? "1" : "0"
    updateProperties(["MTL_HUD_INSIGHTS_ENABLED": value])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetInsightTimeout")
public func MetalPerfKit_SetInsightTimeout(_ seconds: Int32) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    updateProperties(["MTL_HUD_INSIGHT_TIMEOUT": seconds])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetInsightReportInterval")
public func MetalPerfKit_SetInsightReportInterval(_ seconds: Int32) -> Int32 {
    guard getMetalLayer() != nil else {
        return MPHStatus.error.rawValue
    }

    updateProperties(["MTL_HUD_INSIGHT_REPORT_INTERVAL": seconds])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetReportPath")
public func MetalPerfKit_SetReportPath(_ path: UnsafePointer<CChar>?) -> Int32 {
    guard getMetalLayer() != nil, let path = path else {
        return MPHStatus.error.rawValue
    }

    let pathString = String(cString: path)
    updateProperties(["MTL_HUD_REPORT_URL": pathString])

    return MPHStatus.success.rawValue
}

@_cdecl("MetalPerfKit_SetConfigFilePath")
public func MetalPerfKit_SetConfigFilePath(_ path: UnsafePointer<CChar>?) -> Int32 {
    guard getMetalLayer() != nil, let path = path else {
        return MPHStatus.error.rawValue
    }

    let pathString = String(cString: path)
    updateProperties(["MTL_HUD_CONFIG_FILE": pathString])

    return MPHStatus.success.rawValue
}
