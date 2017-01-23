using UnityEngine;

public class Constants
{
    public const int TileCountPerScreen = 1000;
    public const int EnemyLimit = 10;
    public const float BlockChance = 0.1f;
    public const float SkullSpeed = 0.05f;
    public static readonly float CameraDistance = 10 / Mathf.Sqrt(Mathf.Sqrt(TileCountPerScreen));
    public const float CameraSlowness = 0.5f;
    public const float CameraOverviewTime = 1.5f;
    public const float BattleDistanceXFactor = 0.5f;
    public const float BattleDistanceYFactor = 0.5f;
    public const float EntityBattleTransitionSpeed = 0.05f;
    public const float EntityBattleScale = 0.2f;
    public const float PlayerSpeedMultiplier = 3f;
    public const float AttackInfoButtonPadding = 2f;
    public const float AttackMenuAnimationDuration = 0.1f;
    public const float ActionCommandPadding = 2f;
    public const float LevelModifier = 2;
    public const float LerpTransitionSpeed = 1f;
    public const float UILerpTransitionSpeed = 6f;
    public const float ActionCommandHintMagnitude = 0.25f;
    public const float LevelScale = 0.25f;
}