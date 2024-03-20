using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD_HostageScenarioCallout;
using FivePD.API.Utils;
using MenuAPI;

namespace chris_interactionMenu_FivePD;

public class MenuHandler : BaseScript
{
    private Menu _menu;
    private Ped targetPed;

    public MenuHandler()
    {
        _menu = new Menu("Interaction Menu", "Select an option");
        MenuController.AddMenu(_menu);
        Menu _CMEMenu = new Menu("CME", "Commercial Vehicle Enforcement");
        Menu _CMEEnforcementMenu = new Menu("Actions", "Enforcement Actions");
        Menu _PedInteractionMenu = new Menu("Interaction");
        Menu _VehicleSeatsMenu = new Menu("Vehicle Seats");
        Menu _SobrietyMenu = new Menu("Sobriety Tests", "Perform a test");
        List<Menu> allMenus = new List<Menu>()
        {
            _CMEMenu,
            _CMEEnforcementMenu,
            _PedInteractionMenu,
            _VehicleSeatsMenu,
            _SobrietyMenu,
            _menu
        };
        MenuItem[] mainMenu = new[]
        {
            new MenuItem("~o~CME", "Commercial Vehicle Enforcement"), // Leads to CME Menu //
            new MenuItem("~f~Interaction") // Leads to Ped Interaction Menu // 
        };

        MenuItem[] CMEMenu = new[]
        {
            new MenuItem("Check logbook"), // Random boolean (saves)
            new MenuItem("~y~? Enforcement tab ?"), // Circle back with client
        };

        MenuItem[] CMEEnforcementMenu = new[]
        {
            new MenuItem("~r~PLACEHOLDER")
        };

        MenuItem[] PedInteractionMenu = new[]
        {
            new MenuItem("~f~Perform a test", "Sobriety Tests"), // Leads to sobriety menu
            new MenuItem("~g~Grab ped"),
            new MenuItem("~g~Follow"),
            new MenuItem("~g~Seat ped in vehicle"), // Leads to vehicle seats menu and switches with "Unseat ped"
            //new MenuItem("Unseat ped"), // Leads to vehicle occupants menu
            new MenuItem("~f~Vehicle search")
        };
        MenuItem[] VehicleSeatsMenu = new[]
        {
            new MenuItem("Front Driver"),
            new MenuItem("Front Passenger"),
            new MenuItem("Left Rear"),
            new MenuItem("Right Rear")
        };

        MenuItem[] SobrietyMenu = new[]
        {
            new MenuItem("One leg stand"),
            new MenuItem("Finger to nose"),
            new MenuItem("Horizontal gaze"),
            new MenuItem("Counting backwards"),
            new MenuItem("Walk and turn"),
            new MenuItem("Alphabet")
        };

        foreach (var menuItem in mainMenu)
        {
            _menu.AddMenuItem(menuItem);
        }

        MenuController.AddSubmenu(_menu, _CMEMenu);
        MenuController.BindMenuItem(_menu, _CMEMenu, mainMenu[0]);
        foreach (var menuItem in CMEMenu)
        {
            _CMEMenu.AddMenuItem(menuItem);
        }

        _CMEMenu.OnItemSelect += (menu, item, index) =>
        {
            switch (item.Text)
            {
                case "Check logbook":
                {
                    Functions.CheckLogbook(menu, item, index, targetPed);
                    break;
                }
            }
        };
        MenuController.AddSubmenu(_CMEMenu, _CMEEnforcementMenu);
        MenuController.BindMenuItem(_CMEMenu, _CMEEnforcementMenu, CMEMenu[1]);
        foreach (var menuItem in CMEEnforcementMenu)
        {
            _CMEEnforcementMenu.AddMenuItem(menuItem);
        }

        _CMEEnforcementMenu.OnItemSelect += (menu, item, index) =>
        {
            switch (item.Text)
            {
            }
        };

        MenuController.AddSubmenu(_menu, _PedInteractionMenu);
        MenuController.BindMenuItem(_menu, _PedInteractionMenu, mainMenu[1]);
        foreach (var menuItem in PedInteractionMenu)
        {
            _PedInteractionMenu.AddMenuItem(menuItem);
        }

        MenuController.AddSubmenu(_PedInteractionMenu, _SobrietyMenu);
        MenuController.BindMenuItem(_PedInteractionMenu, _SobrietyMenu, PedInteractionMenu[0]);
        foreach (var menuItem in SobrietyMenu)
        {
            _SobrietyMenu.AddMenuItem(menuItem);
        }

        MenuController.AddSubmenu(_PedInteractionMenu, _VehicleSeatsMenu);
        MenuController.BindMenuItem(_PedInteractionMenu, _VehicleSeatsMenu, PedInteractionMenu[1]);
        foreach (var menuItem in VehicleSeatsMenu)
        {
            _VehicleSeatsMenu.AddMenuItem(menuItem);
        }

        bool menuCooldown = false;
        Tick += async () =>
        {
            if (menuCooldown) return;
            if (Game.IsControlJustReleased(0, Control.DropWeapon)) // F9 (56)
            {
                menuCooldown = true;
                Ped closestPed = Utils.GetClosestPed(Game.PlayerPed.Position, 5f);
                if (closestPed != null && closestPed.Exists())
                {
                    if (targetPed != null && closestPed.Position.DistanceTo(Game.PlayerPed.Position) <
                        targetPed.Position.DistanceTo(Game.PlayerPed.Position))
                    {
                        if (Utils.CanEntitySeeEntity(Game.PlayerPed, closestPed))
                            targetPed = closestPed;
                    }
                    else
                    {
                        if (Utils.CanEntitySeeEntity(Game.PlayerPed, closestPed))
                            targetPed = closestPed;
                    }
                }
                if (targetPed == null) return;
                MenuController.MenuAlignmentOption currentAlignment = MenuController.MenuAlignment;
                MenuController.MenuAlignment = MenuController.MenuAlignmentOption.Right;
                _menu.Visible = !_menu.Visible;
                await BaseScript.Delay(500);
                menuCooldown = false;
                while (allMenus.Contains(MenuController.GetCurrentMenu()))
                {
                    API.DrawMarker(27, targetPed.Position.X, targetPed.Position.Y, targetPed.Position.Z - 1f, 0f, 0f,
                        0f, 0f, 0f, 0f, 1f, 1f, 1f, 66, 132, 245, 100, false, false, 2, true, null, null, false);
                    API.DrawMarker(20, targetPed.Position.X, targetPed.Position.Y, targetPed.Position.Z + 1.2f, 0f, 0f,
                        0f, 180f, 0f, 0f, 0.5f, 0.5f, 0.5f, 66, 132, 245, 80, true, true, 2, false, null, null, false);
                    await Delay(0);
                }
                MenuController.MenuAlignment = currentAlignment;
            }
        };
    }
}