using GlobalGameJam2024.Simulation.Procedures;
using System.Text.Json.Serialization;

namespace GlobalGameJam2024.Simulation.Commands;

[JsonDerivedType(typeof(UpdateGameStateClientProcedure), typeDiscriminator: "updategamestate")]
public abstract class ClientProcedure
{
}
