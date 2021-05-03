using System;
using HarmonyLib;
using VoxelTycoon;
using VoxelTycoon.Modding;
using VoxelTycoon.Localization;
using VoxelTycoon.Game.UI.Formatting;

namespace LargeCurrencyNumberAbbreviation
{
   public class LargeCurrencyNumberAbbreviation : Mod {
      private Harmony harmony;

      protected override void Initialize() {
         harmony = new Harmony("com.largecurrencynumberabbreviation.patch");
         harmony.PatchAll();
      }

      protected override void Deinitialize() {
         harmony.UnpatchAll();
      }
   }  
}

public struct Numbers {
   public const double Thousand = 1000d;
   public const double Million = 1000000d;
   public const double Billion = 1000000000d;
   public const double Trillion = 1000000000000d;
   public const double Quadrillion = 1000000000000000d;
}

[HarmonyPatch(typeof(Currency))]
[HarmonyPatch("Format")]
static class CurrencyFormatPatch {
   static bool Prefix() {
      return true;
   }

   static void Postfix(ref string __result, double dollars, Currency __instance){
      string format = (dollars < 0.0) ? __instance.NegativeFormat : __instance.PositiveFormat;
      double covertedNum = __instance.Convert(Math.Abs(dollars));

      double num;
      string suffix;

      if (covertedNum < Numbers.Thousand) {
         suffix = "";
         num = covertedNum;
      } 
      else if (covertedNum < Numbers.Million) {
         suffix = "K";
         num = covertedNum / Numbers.Thousand;
      } 
      else if (covertedNum < Numbers.Billion) {
         suffix = "M";
         num = covertedNum / Numbers.Million;         
      } 
      else if (covertedNum < Numbers.Trillion) {
         suffix = "B";
         num = covertedNum / Numbers.Billion;
      }
      else if (covertedNum < Numbers.Quadrillion) {
         suffix = "T";
         num = covertedNum / Numbers.Trillion;
      } else 
      {
         suffix = "q";
         num = covertedNum / Numbers.Quadrillion;
      }

      string numStr = $"{num.ToString("#0.#")} {suffix}";
      __result = string.Format(format, numStr.ToString(LazyManager<LocaleManager>.Current.Locale.CultureInfo));
   }
}

