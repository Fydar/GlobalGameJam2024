using GlobalGameJam2024.Simulation.ClientCommands;
using System.Text.Json.Serialization;

namespace GlobalGameJam2024.Simulation.Commands;

[JsonDerivedType(typeof(JoinLobbyClientCommand), typeDiscriminator: "join")]
[JsonDerivedType(typeof(MoveClientCommand), typeDiscriminator: "move")]
[JsonDerivedType(typeof(UseAbilityClientCommand), typeDiscriminator: "ability")]
public abstract class ClientCommand
{
}
