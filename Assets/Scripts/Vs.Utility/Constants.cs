namespace Vs.Utility
{
    public static class Constants
    {
        // 게임 설정
        public const int MaxWeaponSlots = 6;
        public const int MaxPassiveSlots = 6;
        public const int MaxWeaponLevel = 8;
        public const int MaxPassiveLevel = 5;
        public const int LevelUpChoices = 3;

        // 레이어
        public const string LayerPlayer = "Player";
        public const string LayerEnemy = "Enemy";
        public const string LayerProjectile = "Projectile";
        public const string LayerPickup = "Pickup";

        // 태그
        public const string TagPlayer = "Player";
        public const string TagEnemy = "Enemy";

        // 경로
        public const string BalancePath = "Balance/";
        public const string WeaponsBalanceFile = "weapons.json";
        public const string PassivesBalanceFile = "passives.json";
        public const string EnemiesBalanceFile = "enemies.json";

        // 풀 설정
        public const int DefaultPoolSize = 20;
        public const int MaxPoolSize = 100;

        // 게임플레이
        public const float DefaultMoveSpeed = 5f;
        public const float PickupRadius = 1.5f;
        public const float ExpMagnetRadius = 3f;

        // 시간
        public const float DefaultStageDuration = 1200f; // 20분
        public const float BossSpawnTime = 1020f; // 17분
    }
}
