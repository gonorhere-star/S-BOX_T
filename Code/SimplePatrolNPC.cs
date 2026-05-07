using Sandbox;
using Sandbox.Citizen;
using System.Collections.Generic;
using System;

public class SimplePatrolNPC : Component
{
    [Property] public float WalkSpeed { get; set; } = 80f;
    [Property] public GameObject Body { get; set; }
    [Property] public List<GameObject> Waypoints { get; set; } = new();
    [Property] public GameObject Player { get; set; }

    [Property] public float WaitTimeMin { get; set; } = 1.5f;
    [Property] public float WaitTimeMax { get; set; } = 4.0f;
    [Property] public float LookAtPlayerDistance { get; set; } = 300f;
    [Property] public bool RandomOrder { get; set; } = true;

    private CharacterController characterController;
    private CitizenAnimationHelper animationHelper;
    private Vector3 WishVelocity;
    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private Random random = new Random();

    protected override void OnAwake()
    {
        characterController = Components.Get<CharacterController>();
        animationHelper = Components.Get<CitizenAnimationHelper>();

        if ( animationHelper != null && Body != null )
        {
            var renderer = Body.GetComponent<SkinnedModelRenderer>();
            if ( renderer != null )
                animationHelper.Target = renderer;
        }

        waitTimer = RandomFloat( WaitTimeMin, WaitTimeMax );
    }

    protected override void OnUpdate()
    {
        if ( ShouldFocusOnPlayer() )
        {
            WishVelocity = Vector3.Zero;
            UpdateAnimations();
            LookAtPlayer();
            return;
        }

        if ( Waypoints.Count == 0 ) return;

        if ( waitTimer > 0f )
        {
            waitTimer -= Time.Delta;
            WishVelocity = Vector3.Zero;
            UpdateAnimations();
            return;
        }

        var targetWaypoint = Waypoints[currentWaypointIndex];
        if ( targetWaypoint == null ) return;

        var direction = (targetWaypoint.WorldPosition - WorldPosition).WithZ( 0 );
        var distance = direction.Length;

        if ( distance < 50f )
        {
            waitTimer = RandomFloat( WaitTimeMin, WaitTimeMax );
            currentWaypointIndex = RandomOrder
                ? random.Next( Waypoints.Count )
                : (currentWaypointIndex + 1) % Waypoints.Count;

            WishVelocity = Vector3.Zero;
            UpdateAnimations();
            return;
        }

        WishVelocity = direction.Normal * WalkSpeed;

        if ( direction.Length > 1f )
        {
            var targetRotation = Rotation.LookAt( direction.Normal, Vector3.Up );
            Body.WorldRotation = Rotation.Lerp( Body.WorldRotation, targetRotation, Time.Delta * 7f );
        }

        UpdateAnimations();
    }

    protected override void OnFixedUpdate()
    {
        if ( WishVelocity.Length > 0.1f )
        {
            characterController.Velocity = characterController.Velocity.WithZ( 0 );
            characterController.Accelerate( WishVelocity );
            characterController.ApplyFriction( 4.0f );
        }
        else
        {
            characterController.Velocity = Vector3.Zero;
        }
        characterController.Move();
    }

    private bool ShouldFocusOnPlayer()
    {
        if ( Player == null ) return false;
        return Vector3.DistanceBetween( WorldPosition, Player.WorldPosition ) <= LookAtPlayerDistance;
    }

    private void LookAtPlayer()
    {
        if ( Player == null ) return;
        var toPlayer = (Player.WorldPosition - WorldPosition).WithZ( 0 );
        if ( toPlayer.Length < 0.1f ) return;

        var lookRotation = Rotation.LookAt( toPlayer.Normal, Vector3.Up );
        Body.WorldRotation = Rotation.Lerp( Body.WorldRotation, lookRotation, Time.Delta * 5f );
    }

    private void UpdateAnimations()
    {
        if ( animationHelper == null ) return;
        animationHelper.WithWishVelocity( WishVelocity );
        animationHelper.WithVelocity( characterController.Velocity );
        animationHelper.IsGrounded = characterController.IsOnGround;
    }

    private float RandomFloat( float min, float max )
    {
        return (float)(random.NextDouble() * (max - min) + min);
    }
}