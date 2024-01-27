using System.Text.Json.Serialization;

namespace GlobalGameJam2024.Simulation.Commands;

[JsonDerivedType(typeof(BroadcastProcedureCommand), typeDiscriminator: "broadcast")]
[JsonDerivedType(typeof(SendProcedureCommand), typeDiscriminator: "send")]
[JsonDerivedType(typeof(LaunchGameHostCommand), typeDiscriminator: "launch")]
public abstract class HostCommand
{
}
