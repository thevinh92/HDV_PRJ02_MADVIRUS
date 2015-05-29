using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

using HDV.MadVirus.Entities.GUI.Controls;
using HDV.FacebookSDK;
using HDV.MadVirus.Constants;
using HDV.FacebookSDK.Models;


namespace HDV.MadVirus.Scenes
{
    public class HomeScene : BaseScene
    {
        const float BackgroundLayerOrder = 1.0f;
        const float LogoLayerOrder = 0.9f;

        #region Item Position Constants
        private static readonly WaveEngine.Common.Math.Vector2 SettingIconPosition = 
            new WaveEngine.Common.Math.Vector2(80, 648);

        private static readonly WaveEngine.Common.Math.Vector2 FacebookIconPosition =
            new WaveEngine.Common.Math.Vector2(1200, 648);

        private static readonly WaveEngine.Common.Math.Vector2 GameMenuPosition = 
            new WaveEngine.Common.Math.Vector2(640, 630);
        #endregion

        #region Menu Item Id
        private const int ContinueGameMenuItemId = 0;
        private const int NewGameMenuItemId = 1;
        private const int OnlinePlayMenuItemId = 2;
        private const int HighscoreMenuItemId = 3;

        private const int MusicItemId = 0;
        private const int SoundItemId = 1;
        private const int InviteItemId = 2;
        private const int HelpItemId = 3;

        private const int FanpageItemId = 4;
        private const int InviteFriendItemId = 5;
        private const int FacebookLogoutItemId = 6;
        #endregion

        private Entity m_Background;
        private Entity m_HomeLogo;
        private FlowerMenu m_SettingMenu;
        private FlowerMenu m_FacebookMenu;
        private GameMenu m_MainMenu;

        protected override void CreateEntities()
        {
            base.CreateEntities();

            //Background
            this.m_Background = new Entity().
                AddComponent(new Transform2D 
                {
                    X = ViewportWidth / 2,
                    Y = ViewportHeight / 2,
                    DrawOrder = BackgroundLayerOrder,
                    Origin = new WaveEngine.Common.Math.Vector2(0.5f)
                }).
                AddComponent(new Sprite("Content/background.wpk")).
                AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            EntityManager.Add(m_Background);

            //Logo
            this.m_HomeLogo = new Entity().
                AddComponent(new Transform2D 
                {
                    X = ViewportWidth / 2,
                    Y = ViewportHeight / 5,
                    DrawOrder = LogoLayerOrder,
                    Origin = new WaveEngine.Common.Math.Vector2(0.5f)
                }).
                AddComponent(new Sprite("Content/home_logo.wpk")).
                AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            EntityManager.Add(m_HomeLogo);

            //Setting  Menu
            this.m_SettingMenu = new FlowerMenu("Content/menus/setting_menu_icon.wpk");
            m_SettingMenu.SetPosition(SettingIconPosition);
            m_SettingMenu.IconMenuItemClicked += OnIconFlowerMenuIconClick;
            m_SettingMenu.MenuItemCheckedChanged += OnMenuItemCheckedChanged;
            m_SettingMenu.AddCheckItem(MusicItemId, "Nhạc nền", 
                "Content/menus/music_on_menu_icon.wpk", 
                "Content/menus/music_off_menu_icon.wpk");
            m_SettingMenu.AddCheckItem(SoundItemId, "Âm thanh", 
                "Content/menus/sound_on_menu_icon.wpk",
                "Content/menus/sound_off_menu_icon.wpk");
            m_SettingMenu.AddCheckItem(InviteItemId, "Có thể nhận được lời mời chơi", 
                "Content/menus/invite_on_menu_icon.wpk",
                "Content/menus/invite_on_menu_icon.wpk");
            m_SettingMenu.AddIconItem(HelpItemId, "Trợ giúp", "Content/menus/help_menu_icon.wpk");

            EntityManager.Add(m_SettingMenu);

            //Facebook Menu
            this.m_FacebookMenu = new FlowerMenu("Content/menus/facebook_menu_icon.wpk");
            m_FacebookMenu.SetPosition(FacebookIconPosition);
            m_FacebookMenu.IconMenuItemClicked += OnIconFlowerMenuIconClick;
            m_FacebookMenu.Clicked += OnFlowMenuClicked;
            m_FacebookMenu.Direction = FlowMenuDirections.UppLeft;
            m_FacebookMenu.AddIconItem(FanpageItemId, "Fanpage", "Content/menus/fanpage_menu_icon.wpk");
            m_FacebookMenu.AddIconItem(InviteFriendItemId, "Mời bạn chơi trên Facebook", 
                "Content/menus/invite_facebook_menu_icon.wpk");
            m_FacebookMenu.AddIconItem(FacebookLogoutItemId, "Đăng xuất Facebook", 
                "Content/menus/facebook_logout_menu_icon.wpk");

            EntityManager.Add(m_FacebookMenu);

            //Game Menu
            this.m_MainMenu = new GameMenu();
            m_MainMenu.MenuItemClicked += OnGameMenuItemClick;
            m_MainMenu.AddItem(ContinueGameMenuItemId, "Tiếp tục");
            m_MainMenu.AddItem(NewGameMenuItemId, "Chơi mới");
            m_MainMenu.AddItem(OnlinePlayMenuItemId, "Đấu Online");
            m_MainMenu.AddItem(HighscoreMenuItemId, "Điểm cao");

            m_MainMenu.SetPosition(GameMenuPosition);

            EntityManager.Add(m_MainMenu);

        }

        private async void OnFlowMenuClicked(object sender, FlowerMenuClickEventArgs e)
        {
            if (!FacebookClient.Current.IsLoggedIn)
            {
                e.IsCancelShowItems = true;
                try
                {
                    var session = await FacebookClient.Current.LoginAsync(Configuration.FacebookAppId,
                        Permission.public_profile,
                        Permission.publish_actions,
                        Permission.user_friends,
                        Permission.user_games_activity);

                    m_FacebookMenu.ShowItems();
                } 
                catch
                {
                    System.Diagnostics.Debug.WriteLine("Co loi xay ra");
                }
                return;
            }
        }

        private void OnMenuItemCheckedChanged(object sender, FlowerMenuItemCheckedChangedEventArgs e)
        {
        }

        private async void OnIconFlowerMenuIconClick(object sender, FlowerMenuItemClickEventArgs e)
        {
            try
            {
                IconFlowerMenuItem iconMenuItem = e.ClickedItem as IconFlowerMenuItem;
                if (iconMenuItem == null)
                    return;

                switch (iconMenuItem.Id)
                {
                    case FacebookLogoutItemId:
                        await FacebookClient.Current.LogoutAsync();
                        break;

                    case FanpageItemId:
                        var applicationInfo = await FacebookClient.Current.GetApplicationInfoAsync();
                        if (!string.IsNullOrEmpty(applicationInfo.Link))
                        {
#if WINDOWS
                            System.Diagnostics.Process.Start(applicationInfo.Link);
#else
#endif
                        }
                        break;
                }
            }
            catch
            {

            }
        }

        private void OnGameMenuItemClick(object sender, GameMenuItemClickedEventArgs e)
        {
            int itemId = e.ClickedItem.Id;
            switch (itemId)
            {
                case NewGameMenuItemId:
                    // Modify code here
                    // Input with a bool value
                    WaveServices.ScreenContextManager.Push(
                        new ScreenContext(new GamePlayScene()));
                    break;

                case ContinueGameMenuItemId:
                    WaveServices.ScreenContextManager.Push(
                        new ScreenContext(new GamePlayScene()));
                    break;

                case OnlinePlayMenuItemId:
                    break;

                case HighscoreMenuItemId:
                    WaveServices.ScreenContextManager.Push(
                        new ScreenContext(new HighscoreScene()));
                    break;
            }
        }
    }
}
