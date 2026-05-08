using Sandbox;
using Sandbox.Citizen;

public class PlayerMovement : Component
{
    [Property] public float WalkSpeed { get; set; } = 150f;
    [Property] public float RunSpeed { get; set; } = 300f;
    [Property] public float JumpForce { get; set; } = 400f;
    [Property] public float GroundControl { get; set; } = 4.0f;
    [Property] public float AirControl { get; set; } = 0.1f;
    [Property] public float BodyRotationSpeed { get; set; } = 15f;

    [Property] public GameObject Body { get; set; }
    [Property] public GameObject CameraObject { get; set; }

    private CharacterController characterController;
    private CitizenAnimationHelper animationHelper;
    private Vector3 WishVelocity;

    protected override void OnAwake()
    {
        characterController = Components.Get<CharacterController>();

        animationHelper = Components.Get<CitizenAnimationHelper>();
        if (animationHelper == null && Body != null)
            animationHelper = Body.GetComponent<CitizenAnimationHelper>();

        if (animationHelper != null && Body != null)
        {
            var renderer = Body.GetComponent<SkinnedModelRenderer>();
            if (renderer != null)
                animationHelper.Target = renderer;
        }
    }

    protected override void OnUpdate()
    {
        BuildWishVelocity();
        RotateBody();
        UpdateAnimations();
        // Никаких проверок инвентаря!
    }

    protected override void OnFixedUpdate() => Move();

    private void BuildWishVelocity()
    {
        var moveDir = Input.AnalogMove;
        if (CameraObject == null) return;

        var cameraRotation = CameraObject.WorldRotation;
        WishVelocity = cameraRotation * moveDir;

        var speed = Input.Down("Run") ? RunSpeed : WalkSpeed;
        WishVelocity *= speed;
    }

    private void RotateBody()
    {
        if (WishVelocity.Length <= 0.1f) return;
        if (Body == null) return;

        var horizontalDir = WishVelocity.WithZ(0).Normal;
        if (horizontalDir.Length < 0.01f) return;

        var targetRotation = Rotation.LookAt(horizontalDir, Vector3.Up);
        Body.WorldRotation = Rotation.Lerp(Body.WorldRotation, targetRotation, Time.Delta * BodyRotationSpeed);
    }

    private void Move()
    {
        var gravity = Scene.PhysicsWorld.Gravity;

        if (characterController.IsOnGround)
        {
            characterController.Velocity = characterController.Velocity.WithZ(0);
            characterController.Accelerate(WishVelocity);
            characterController.ApplyFriction(GroundControl);
            if (Input.Pressed("Jump"))
                characterController.Punch(Vector3.Up * JumpForce);
        }
        else
        {
            characterController.Velocity += gravity * Time.Delta * 0.5f;
            characterController.Accelerate(WishVelocity.ClampLength(100f));
            characterController.ApplyFriction(AirControl);
        }

        characterController.Move();

        if (!characterController.IsOnGround)
            characterController.Velocity += gravity * Time.Delta * 0.5f;
        else
            characterController.Velocity = characterController.Velocity.WithZ(0);
    }

    private void UpdateAnimations()
    {
        if (animationHelper == null) return;

        animationHelper.WithWishVelocity(WishVelocity);
        animationHelper.WithVelocity(characterController.Velocity);
        animationHelper.IsGrounded = characterController.IsOnGround;
    }
}