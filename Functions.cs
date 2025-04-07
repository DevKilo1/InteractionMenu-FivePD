using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using FivePD_HostageScenarioCallout;
using MenuAPI;

namespace chris_interactionMenu_FivePD;

public class Functions
{
    private static Dictionary<int, bool> LogbookValidity = new Dictionary<int, bool>();
    public static bool CheckLogbook(Menu _menu, MenuItem menuItem, int i, Ped ped)
    {
        if (LogbookValidity.ContainsKey(ped.NetworkId))
        {
            var res = LogbookValidity[ped.NetworkId];
            string validity = !res ? "~r~Invalid~s~" : "~g~Valid~s~";
            Utils.ShowNetworkedNotification($"Logbook: {validity}", "~o~CME Inspection", "~m~Logbook Checker");
            return res;
        }
        else
        {
            var chance = new Random().Next(1, 100);
            string validity = chance <= 50 ? "~r~Invalid~s~" : "~g~Valid~s~";
            bool validBool = validity == "~g~Valid~s~";
            Utils.ShowNetworkedNotification($"Logbook: {validity}", "~o~CME Inspection", "~m~Logbook Checker");
            LogbookValidity.Add(ped.NetworkId, validBool);
            return validBool;
        }
    }

    public static async Task<bool> SeatPedInVehicle(Ped ped, Vehicle vehicle, VehicleSeat seat, bool teleportIn = false)
    {
        bool result = false;
        if (ped == null || vehicle == null) return false;

        if (teleportIn)
            ped.SetIntoVehicle(vehicle, seat);
        else
        {
            ped.Task.EnterVehicle(vehicle, seat);
            await Utils.KeepTaskEnterVehicle(ped, vehicle, seat);
        }

        if (ped.IsSittingInVehicle(vehicle) && ped.SeatIndex == seat)
            result = true;

        return result;
    } 
}