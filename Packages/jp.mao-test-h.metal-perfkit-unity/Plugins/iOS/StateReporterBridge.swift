import Foundation

#if canImport(StateReporting)
import StateReporting
#endif

private enum StateReporterBridgeStatus: Int32 {
    case success = 1
    case error = -1
}

private enum StateReporterMetadataType: Int, Decodable {
    case string = 0
    case boolean = 1
    case integer = 2
    case floatingPoint = 3
    case date = 4
}

private struct StateReporterMetadataEntry: Decodable {
    let key: String
    let type: StateReporterMetadataType
    let value: String
}

private struct StateReporterMetadataPayload: Decodable {
    let entries: [StateReporterMetadataEntry]
}

private enum StateReporterBridgeError: Error {
    case invalidMetadata
}

#if canImport(StateReporting)
@available(iOS 27.0, macOS 27.0, *)
private extension StateReporterMetadataEntry {
    func reportableValue() throws -> ReportableMetadataValue {
        switch type {
        case .string:
            return ReportableMetadataValue(value)
        case .boolean:
            if value == "true" {
                return ReportableMetadataValue(true)
            }
            if value == "false" {
                return ReportableMetadataValue(false)
            }
            throw StateReporterBridgeError.invalidMetadata
        case .integer:
            guard let integer = Int128(value) else {
                throw StateReporterBridgeError.invalidMetadata
            }
            return .integer(integer)
        case .floatingPoint:
            guard let floatingPoint = Double(value) else {
                throw StateReporterBridgeError.invalidMetadata
            }
            return ReportableMetadataValue(floatingPoint)
        case .date:
            guard let milliseconds = Int64(value) else {
                throw StateReporterBridgeError.invalidMetadata
            }
            return ReportableMetadataValue(
                Date(timeIntervalSince1970: Double(milliseconds) / 1_000.0))
        }
    }
}

@available(iOS 27.0, macOS 27.0, *)
private struct MetalPerfKitStateMetadata: ReportableMetadata {
    let metadataDictionary: [String: ReportableMetadataValue]
}

@available(iOS 27.0, macOS 27.0, *)
private func decodeMetadata(
    _ metadataJson: UnsafePointer<CChar>?
) throws -> MetalPerfKitStateMetadata? {
    guard let metadataJson else {
        return nil
    }

    let data = Data(String(cString: metadataJson).utf8)
    let payload = try JSONDecoder().decode(StateReporterMetadataPayload.self, from: data)
    var metadataDictionary: [String: ReportableMetadataValue] = [:]
    for entry in payload.entries {
        metadataDictionary[entry.key] = try entry.reportableValue()
    }

    return MetalPerfKitStateMetadata(metadataDictionary: metadataDictionary)
}

@available(iOS 27.0, macOS 27.0, *)
private func reportTransition(
    domain: UnsafePointer<CChar>?,
    stateLabel: UnsafePointer<CChar>?,
    stableMetadataJson: UnsafePointer<CChar>?,
    volatileMetadataJson: UnsafePointer<CChar>?
) -> Int32 {
    guard let domain else {
        return StateReporterBridgeStatus.error.rawValue
    }

    let domainString = String(cString: domain)
    guard !domainString.isEmpty else {
        return StateReporterBridgeStatus.error.rawValue
    }

    let stateLabelString = stateLabel.map(String.init(cString:))
    guard stateLabelString?.isEmpty != true else {
        return StateReporterBridgeStatus.error.rawValue
    }

    do {
        let stableMetadata = try decodeMetadata(stableMetadataJson)
        let volatileMetadata = try decodeMetadata(volatileMetadataJson)
        let reporter = StateReporter<MetalPerfKitStateMetadata, MetalPerfKitStateMetadata>.reporter(
            for: domainString,
            stableMetadata: MetalPerfKitStateMetadata.self,
            volatileMetadata: MetalPerfKitStateMetadata.self)
        reporter.reportTransition(
            to: stateLabelString,
            stableMetadata: stableMetadata,
            volatileMetadata: volatileMetadata)
        return StateReporterBridgeStatus.success.rawValue
    } catch {
        return StateReporterBridgeStatus.error.rawValue
    }
}

@available(iOS 27.0, macOS 27.0, *)
private func reportVolatileMetadataUpdate(
    domain: UnsafePointer<CChar>?,
    updatedMetadataJson: UnsafePointer<CChar>?
) -> Int32 {
    guard let domain else {
        return StateReporterBridgeStatus.error.rawValue
    }

    let domainString = String(cString: domain)
    guard !domainString.isEmpty else {
        return StateReporterBridgeStatus.error.rawValue
    }

    do {
        let updatedMetadata = try decodeMetadata(updatedMetadataJson)
        let reporter = StateReporter<MetalPerfKitStateMetadata, MetalPerfKitStateMetadata>.reporter(
            for: domainString,
            stableMetadata: MetalPerfKitStateMetadata.self,
            volatileMetadata: MetalPerfKitStateMetadata.self)
        reporter.reportVolatileMetadataUpdate(updatedMetadata)
        return StateReporterBridgeStatus.success.rawValue
    } catch {
        return StateReporterBridgeStatus.error.rawValue
    }
}
#endif

@_cdecl("MetalPerfKit_StateReporter_ReportTransition")
public func MetalPerfKit_StateReporter_ReportTransition(
    _ domain: UnsafePointer<CChar>?,
    _ stateLabel: UnsafePointer<CChar>?,
    _ stableMetadataJson: UnsafePointer<CChar>?,
    _ volatileMetadataJson: UnsafePointer<CChar>?
) -> Int32 {
#if canImport(StateReporting)
    if #available(iOS 27.0, macOS 27.0, *) {
        return reportTransition(
            domain: domain,
            stateLabel: stateLabel,
            stableMetadataJson: stableMetadataJson,
            volatileMetadataJson: volatileMetadataJson)
    }
#endif
    return StateReporterBridgeStatus.success.rawValue
}

@_cdecl("MetalPerfKit_StateReporter_ReportVolatileMetadataUpdate")
public func MetalPerfKit_StateReporter_ReportVolatileMetadataUpdate(
    _ domain: UnsafePointer<CChar>?,
    _ updatedMetadataJson: UnsafePointer<CChar>?
) -> Int32 {
#if canImport(StateReporting)
    if #available(iOS 27.0, macOS 27.0, *) {
        return reportVolatileMetadataUpdate(
            domain: domain,
            updatedMetadataJson: updatedMetadataJson)
    }
#endif
    return StateReporterBridgeStatus.success.rawValue
}
