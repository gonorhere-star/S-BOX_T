using Sandbox;

public class ThirdPersonCamera : Component
{
    [Property] public GameObject Target { get; set; }
    [Property] public Vector3 TargetOffset { get; set; } = new Vector3(0, 0, 64f);
    [Property] public float Distance { get; set; } = 200f;
    [Property] public float Sensitivity { get; set; } = 0.1f;
    [Property] public float HeightOffset { get; set; } = 20f;

    [Property] public bool InvertY { get; set; } = false;
    [Property] public bool InvertX { get; set; } = false;   // <-- добавили

    private Angles eyeAngles;

    protected override void OnStart()
    {
        eyeAngles = GameObject.WorldRotation.Angles();
    }

    protected override void OnUpdate()
    {
        // Инвертирование обоих осей по необходимости
        float mouseX = Input.MouseDelta.x * (InvertX ? -1f : 1f);
        float mouseY = Input.MouseDelta.y * (InvertY ? 1f : -1f);

        eyeAngles.pitch -= mouseY * Sensitivity;
        eyeAngles.yaw   += mouseX * Sensitivity;
        eyeAngles.pitch = eyeAngles.pitch.Clamp(-89.9f, 89.9f);
        eyeAngles.roll  = 0f;

        GameObject.WorldRotation = eyeAngles.ToRotation();

        if (Target == null) return;

        Vector3 targetPos = Target.WorldPosition + TargetOffset;
        Vector3 backward = eyeAngles.ToRotation().Backward;
        Vector3 up = eyeAngles.ToRotation().Up;
        GameObject.WorldPosition = targetPos + backward * Distance + up * HeightOffset;
    }
}