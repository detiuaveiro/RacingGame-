using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JamCat.UI 
{
    public class Window_CharacterSelection : Window
    {
        // Variables -> Instance
        public static Window_CharacterSelection instance;
        public static Window_CharacterSelection Get() { return instance; }

        // Variables
        public UI_ToggleGroup toggleGroup;

        // Methods -> Override
        protected override void OnAwakeWindow() {
            instance = this;
        }

        protected override void OnUpdateWindow() {

        }

        protected override void OnOpenWindow() {

        }

        protected override void OnCloseWindow() {

        }

        // Methods -> Public
        public void ButtonHost() {
            Button_Play();
            ManagerServer.Get().StartHost();
        }

        public void ButtonJoin() {
            Button_Play();
            ManagerServer.Get().StartClient();
        }

        public void Button_Play() {
            if (Data.Get().gameData.character_selected < 0)
                return;

            CloseWindow(0.2f, 0);
            Window_HUD.Get().OpenWindow(0.2f, 0.2f);
            GeneralMethods.StartGame();
        }

        public void Button_Select_Character(int number) {
            Data.Get().gameData.character_selected = number;
        }
        
        public void Button_Back() {
            Data.Get().gameData.character_selected = -1;
            CloseWindow(0.5f, 0);
            Window_MainMenu.Get().OpenWindow(0.5f, 0.5f);
        }
    }
}