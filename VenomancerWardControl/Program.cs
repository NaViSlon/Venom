using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using SharpDX;
using System;
using System.Linq;
using System.Windows.Input;




namespace Poisonous_Daddy
{
    internal class Program
    {
        #region

        private static readonly uint[] PlagueWardDamage = { 10, 19, 29, 38 };
        private static void Main()
        {
            Game.OnUpdate += Game_OnUpdate;
            
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            var me = ObjectMgr.LocalHero;

            if (!Game.IsInGame && me == null)
                return;

            if (me.ClassID != ClassID.CDOTA_Unit_Hero_Venomancer)
                return;

            var Level = me.Spellbook.SpellE.Level - 1;
            var enemies = ObjectMgr.GetEntities<Hero>().Where(hero => hero.IsAlive && !hero.IsIllusion && hero.IsVisible && hero.Team != me.Team).ToList();
            var creeps = ObjectMgr.GetEntities<Creep>().Where(creep => (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege) && creep.IsAlive && creep.IsVisible && creep.IsSpawned && creep.Health <= ((int)PlagueWardDamage[Level] * (1 - creep.DamageResist) + 30)).ToList();
            var wards = ObjectMgr.GetEntities<Unit>().Where(unit => unit.ClassID == ClassID.CDOTA_BaseNPC_Venomancer_PlagueWard && unit.IsAlive && unit.CanAttack()).ToList();
            var notarget = ObjectMgr.GetEntities<Creep>().Where(creep => (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane || creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege) && creep.IsAlive && creep.IsVisible && creep.IsSpawned && creep.Health > ((int)PlagueWardDamage[Level] * (1 - creep.DamageResist) + 30)).ToList();

            if (!enemies.Any() || !creeps.Any() || !wards.Any() || !(Level > 0))
                return;
        #endregion

        #region enemy
            foreach (var enemy in enemies)
            {
                if (enemy.Health > 0)
                {
                    foreach (var v in wards)
                    {
                        if (GetDistance2D(enemy.Position, v.Position) < v.AttackRange && Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(enemy.Player.Hero);
                            Utils.Sleep(200, v.Handle.ToString());
                        }
                    }
                }
            }
            #endregion 

        #region creeps
            foreach (var creep in creeps)
            {
                foreach (var v in wards)
                {
                    
                        #region enemycreeps
                        if (creep.Team != me.Team && creep.Health <= (PlagueWardDamage[Level] * (1 - creep.DamageResist) + 30)
                                && GetDistance2D(creep.Position, v.Position) < v.AttackRange)
                        {

                            if (GetDistance2D(creep.Position, v.Position) < v.AttackRange && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                v.Attack(creep);
                                Utils.Sleep(1000, v.Handle.ToString());
                            }
                        }
                    
                    #endregion

                        #region mycreeps
                        if (creep.Team == me.Team && creep.Health <= (PlagueWardDamage[Level] * (1 - creep.DamageResist) + 30))
                    {
                        if (GetDistance2D(creep.Position, v.Position) < v.AttackRange && Utils.SleepCheck(v.Handle.ToString()))
                        {
                            v.Attack(creep);
                            Utils.Sleep(1000, v.Handle.ToString());
                        }
                    }
                        #endregion

                        #region notarget
                        foreach (var t in notarget)
                        {
                            if (GetDistance2D(t.Position, v.Position) < v.AttackRange && Utils.SleepCheck(v.Handle.ToString()))
                            {
                                //v.Stop();
								v.Hold();
                                Utils.Sleep(100, v.Handle.ToString());
                            }
                        }
                    #endregion
                }

            }
#endregion
        }
                
            
       
        private static float GetDistance2D(Vector3 p1, Vector3 p2)
        {
            return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        
    }
}
