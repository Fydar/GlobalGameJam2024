using System.Text.Json.Serialization;

namespace GlobalGameJam2024.Simulation.Commands;

[JsonDerivedType(typeof(IntakeClientCommandHostProcedure), typeDiscriminator: "intake")]
[JsonDerivedType(typeof(PlayerJoinedHostProcedure), typeDiscriminator: "joined")]
[JsonDerivedType(typeof(PlayerLeftHostProcedure), typeDiscriminator: "left")]
public abstract class HostProcedure
{
}
