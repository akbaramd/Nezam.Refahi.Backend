namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Elevator
{
    public int Id { get; set; }

    public int ElectricalSpecFormId { get; set; }

    public string? ElevatorType { get; set; }

    public int ElevatorPersionCap { get; set; }

    public float ElevatorWeightCap { get; set; }

    public float ElevatorLength { get; set; }

    public float LowSpeed { get; set; }

    public float FastSpeed { get; set; }

    public int StopCount { get; set; }

    public string? DoorType { get; set; }

    public float DoorUsefulWidth { get; set; }

    public float DoorUsefulHight { get; set; }

    public string? MechanicalDoorLock { get; set; }

    public string? LockBuilder { get; set; }

    public string? LockType { get; set; }

    public string? SpeedGovernorBuilder { get; set; }

    public string? SpeedGoverType { get; set; }

    public string? SafetyBrakeBuilder { get; set; }

    public string? SafetyBrakeType { get; set; }

    public string? StopLength { get; set; }

    public string? MotorBuilder { get; set; }

    public string? MotorSerialNumber { get; set; }

    public string? MotorType { get; set; }

    public string? StartPerHour { get; set; }

    public string? Power { get; set; }

    public string? Voltage { get; set; }

    public string? Current { get; set; }

    public string? LowRpm { get; set; }

    public string? FastRpm { get; set; }

    public string? GearboxType { get; set; }

    public string? GearboxBuilder { get; set; }

    public string? GearboxConversionRate { get; set; }

    public string? BrakeType { get; set; }

    public string? Description { get; set; }

    public bool ExecutionQuality { get; set; }

    public double? MechanicalStopSpeed { get; set; }

    public bool StandardElevatorEquipment { get; set; }

    public virtual ElectricalSpecForm ElectricalSpecForm { get; set; } = null!;
}
