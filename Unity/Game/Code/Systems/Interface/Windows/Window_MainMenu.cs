using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace JamCat.UI 
{
    public class Window_MainMenu : Window
    {
        // Variables -> Instance
        public static Window_MainMenu instance;
        public static Window_MainMenu Get() { return instance; }

        // Variables
        private bool quit;
        public Window windowHiddenInfoCV;
        public Window windowMainMenu;
        public Animator animatorOnPlay;

        // Methods -> Override
        protected override void OnAwakeWindow() {
            instance = this;
        }

        protected override void OnOpenWindow() {
            
        }
        
        protected override void OnCloseWindow() {

        }

        protected override void OnUpdateWindow() {

            if (Input.GetKeyDown(KeyCode.O)) {
                windowHiddenInfoCV.Toggle();

                if (windowHiddenInfoCV.is_opened == true) 
                    windowMainMenu.CloseWindow(0.3f, 0);
                else 
                    windowMainMenu.OpenWindow(0.3f, 0);
            }

            if (quit == true) {
                Window_Fade.Get().CloseWindow(1, 0);
                if (Window_Fade.Get().canvasGroup.alpha <= 0)
                    Application.Quit();
            }
        }

        // Methods -> Public
        public void Button_Play() {
            CloseWindow(0.2f, 0f);
            Window_Lobby.Get().OpenWindow(0.2f, 0.2f);
            animatorOnPlay.SetBool("Opening", true);
        }

        public void Button_Options() {
            CloseWindow(0.2f, 0f);
            Window_Options.Get().OpenWindow(0.2f, 0.2f);
            Window_Options.Get().StartOptions(this);
        }

        public void Button_Credits() {
            CloseWindow(0.2f, 0f);
            Window_Credits.Get().OpenWindow(0.2f, 0.2f);
        }

        public void Button_Quit() {
            quit = true;
            Application.Quit();
        }
    }
}