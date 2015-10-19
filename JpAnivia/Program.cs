using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;

namespace anivia
{

    class Program
    {
        private static AIHeroClient myHero { get { return ObjectManager.Player; } }
        public static Spell.Skillshot Q, W, R;
        public static Spell.Targeted E;
        public static Menu Menu, SettingsMenu, HarassMenu;
        private static GameObject QMissile, RMissile;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;

        }

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (_Player.ChampionName != "Anivia")
                return;
            Q = new Spell.Skillshot(SpellSlot.Q, 1200, SkillShotType.Linear);

            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Circular);
            E = new Spell.Targeted(SpellSlot.E, 650);
            R = new Spell.Skillshot(SpellSlot.R, 650, SkillShotType.Circular);

            Menu = MainMenu.AddMenu("jpanivia", "jpAnivia");
            Menu.AddGroupLabel("jpAnivia  V1.0");
            Menu.AddSeparator();
            Menu.AddLabel("Made jairp29");

            SettingsMenu = Menu.AddSubMenu("Settings", "Settings");
            SettingsMenu.AddGroupLabel("Settings");
            SettingsMenu.AddLabel("Combo");
            SettingsMenu.Add("Qcombo", new CheckBox("Use Q on Combo"));
            SettingsMenu.Add("Wcombo", new CheckBox("Use W on Combo"));
            SettingsMenu.Add("Rcombo", new CheckBox("use R on combo"));
            SettingsMenu.Add("Ecombo", new CheckBox("Use E on Combo"));
            SettingsMenu.Add("Qdraw", new CheckBox("draw Q"));
            SettingsMenu.Add("Wdraw", new CheckBox("draw W"));
            SettingsMenu.Add("Edraw", new CheckBox("draw E"));
            SettingsMenu.Add("Rdraw", new CheckBox("draw R"));
            SettingsMenu.AddLabel("LANE CLEAR");
            SettingsMenu.Add("Qlc", new CheckBox("Use Q"));
            SettingsMenu.Add("minione", new Slider("Minions to use E", 3, 1, 20));
            SettingsMenu.Add("mana.lane", new Slider("Mim % Mana", 50, 0, 100));
            SettingsMenu.AddLabel("JUNGLE CLEAR");
            SettingsMenu.Add("Qjg", new CheckBox("Use Q", true));
            SettingsMenu.Add("Wjg", new CheckBox("Use W", true));

            HarassMenu = Menu.AddSubMenu("Harass", "HarassAnivia");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.AddSeparator();
            HarassMenu.Add("RoffHarass", new CheckBox("Use R"));
            HarassMenu.Add("useQHarass", new CheckBox("Use Q"));


           
            GameObject.OnCreate += OnCreateObj;
            GameObject.OnDelete += OnDeleteObj;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnTick += Game_OnTick;
            Game.OnTick += Game_OnTick1;
        }

        private static void OnCreateObj(GameObject sender, EventArgs args)
        {
            if (sender != null && sender.Name != null)
            {

                if (sender.Name == "cryo_FlashFrost_Player_mis.troy")
                    QMissile = sender;
                if (sender.Name.Contains("cryo_storm"))
                    RMissile = sender;

            }
        }

        private static void OnDeleteObj(GameObject sender, EventArgs args)
        {
            if (sender != null && sender.Name != null)
            {
                if (sender.Name == "cryo_FlashFrost_Player_mis.troy")
                    QMissile = null;
                if (sender.Name.Contains("cryo_storm"))
                    RMissile = null;
            }
        }



        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
              
            }
        }

        private static void Game_OnTick1(EventArgs args)
        {
           
     




            
            if ( W.IsReady())
            {
                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsEnemy).Where(x => !x.IsDead).Where(x => _Player.Distance(x) <= W.Range))
                {

                    if (_Player.Distance(enemy) > 900)
                    {

                        var condemnPos = enemy.Position.Extend(_Player.Position, enemy.Distance(_Player) - 470);
                        var realStart = Drawing.WorldToScreen(enemy.Position);
                        var realEnd = Drawing.WorldToScreen(condemnPos.To3D());

                       
                        new Circle() { Color = Color.Blue, Radius = 60, BorderWidth = 2f }.Draw(condemnPos.To3D());

                    }

                    
 

                }
            }   
            
           
            
        }



        private static void Combo()
        {
            var useQ = SettingsMenu["Qcombo"].Cast<CheckBox>().CurrentValue;
            var useW = SettingsMenu["Wcombo"].Cast<CheckBox>().CurrentValue;
            var useE = SettingsMenu["Ecombo"].Cast<CheckBox>().CurrentValue;
            var useR = SettingsMenu["Rcombo"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            var condemnPos = _Player.Position.Extend(target.Position, _Player.Distance(target) + 150);
           



            if (target == null)


                return;

            if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && !target.IsZombie && QMissile == null)
            {
                Q.Cast(target);
            }

            if (useQ && Q.IsReady() && QMissile.Position.CountEnemiesInRange(230) > 0)
            {
                Q.Cast(target);
            }

            if (Q.IsOnCooldown && E.IsReady() && target.IsValidTarget(E.Range) && !target.IsDead && !target.IsZombie)
            {

                E.Cast(target);
            }


            if (Q.IsOnCooldown && R.IsReady() && target.IsValidTarget(R.Range) && !target.IsDead && !target.IsZombie && RMissile == null)
            {
                R.Cast(target);
            }

            if (RMissile != null && RMissile.Position.CountEnemiesInRange(400) == 0)
            {

                R.Cast(target);
            }

            if (E.IsOnCooldown && W.IsReady() && target.IsValidTarget(W.Range) && _Player.Distance(target.ServerPosition) > 600)


            {
               


                  W.Cast(condemnPos.To3D());


                
            }




        }









        private static void Drawing_OnDraw(EventArgs args)
        {
            var useQ = SettingsMenu["QDraw"].Cast<CheckBox>().CurrentValue;
            var useW = SettingsMenu["WDraw"].Cast<CheckBox>().CurrentValue;
            var useE = SettingsMenu["EDraw"].Cast<CheckBox>().CurrentValue;
            var useR = SettingsMenu["RDraw"].Cast<CheckBox>().CurrentValue;

            if (useQ && Q.IsReady())
            {
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = Q.Range }.Draw(_Player.Position);
            }

            if (useE && E.IsReady())
            {
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = E.Range }.Draw(_Player.Position);
            }

            


            if (useR && R.IsReady())
            {
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = R.Range }.Draw(_Player.Position);
            }

        }


        
    }
}