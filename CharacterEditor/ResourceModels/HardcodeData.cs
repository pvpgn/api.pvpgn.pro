using System;
using System.Collections.Generic;
using System.Text;

namespace CharacterEditor.ResourceModels
{
    /// <summary>
    /// TODO: data class which is required to finish https://github.com/pvpgn/api.pvpgn.pro/issues/1#issuecomment-552119449
    /// </summary>
    public static class HardcodedData
    {
        /* All values in this file have been pulled from the 'Arreat Summit',
           Blizzard's unofficial Diablo II fansite (http://www.battle.net/diablo2exp/)
           and have been checked and rechecked with debug output functions. */

        /* What 'type' entries correspond to a helm. */
        public static readonly int sizeof_helm_tags = 4;
        public static readonly string[] helm_tags = { "helm", "pelt", "phlm", "circ" };

        /* What 'type' entries correspond to a shield. */
        public static readonly int sizeof_shield_tags = 3;
        public static readonly string[] shield_tags = { "shie", "ashd", "head" };

        /* This is a value lookup table for certain properties of gems. */
        public static readonly int[] val1_lookup = {
          2, 2, 3, 3, 4,
          4, 8, 12, 16, 20,
          2, 3, 3, 4, 5,
          40, 60, 80, 100, 150,
          8, 12, 18, 24, 30,
          3, 4, 6, 8, 10,
          1, 3, 4, 6, 10,
          12, 16, 22, 28, 40,
          10, 17, 24, 31, 38,
          10, 20, 40, 60, 100,
          12, 16, 22, 28, 40,
          3, 4, 6, 8, 10,
          3, 5, 8, 10, 15,
          12, 16, 22, 28, 40,
          10, 17, 24, 31, 38,
          28, 34, 44, 54, 68,
          6, 8, 11, 14, 19,
          20, 40, 60, 80, 100,
          1, 1, 1, 1, 1,
          12, 16, 22, 28, 40,
          9, 13, 16, 20, 24 };

        /* This is a second value lookup table in case the gem has two or more
           properties (a couple do). */
        public static readonly int[] val2_lookup = {
          1, 2, 2, 3, 3,
          0, 0, 0, 0, 0,
          8, 8, 12, 12, 19,
          0, 0, 0, 0, 0,
          0, 0, 0, 0, 0,
          0, 0, 0, 0, 0,
          3, 5, 7, 10, 14,
          0, 0, 0, 0, 0,
          0, 0, 0, 0, 0,
          10, 20, 40, 60, 100,
          0, 0, 0, 0, 0,
          0, 0, 0, 0, 0,
          4, 8, 12, 16, 20,
          0, 0, 0, 0, 0,
          0, 0, 0, 0, 0,
          0, 0, 0, 0, 0,
          0, 0, 0, 0, 0,
          0, 0, 0, 0, 0,
          8, 14, 22, 30, 40,
          0, 0, 0, 0, 0,
          0, 0, 0, 0, 0 };

        /* Every gem type has the same property ID types, so this is a lookup
           table for the first property. */
        public static readonly int[] id_lookup = {
          60, 78, 74,
          19, 31, 0,
          54, 43, 9,
          57, 45, 2,
          48, 39, 7,
          122, 39, 19,
          50, 41, 80 };

        /* This is a lookup table for rune property values. */
        public static readonly int[] rune_val1_lookup = {
          1, 1, 1,
          50, 7, 15,
          2, 2, 2,
          0, 30, 30,
          -25, 15, 15,
          9, 15, 15,
          75, 35, 30,
          5, 35, 30,
          1, 35, 30,
          3, 35, 30,
          7, 14, 14,
          9, 7, 7,
          20, 20, 20,
          25, 7, 7,
          -20, -15, -15,
          10, 10, 10,
          10, 10, 10,
          10, 10, 10,
          10, 10, 10,
          75, 50, 50,
          100, 30, 30,
          25, 22, 15,
          0, 7, 7,
          30, 25, 25,
          20, 5, 5,
          7, 5, 5,
          50, 5, 5,
          20, 5, 5,
          1, 50, 5,
          20, 8, 8,
          0, 50, 5,
          3, 0, 0,
          0, 0, 0 };

        /* This is a lookup table for rune property IDs. */
        public static readonly int[] rune_id_lookup = {
          89, 89, 89,
          124, 20, 154,
          138, 138, 138,
          81, 32, 32,
          116, 27, 27,
          22, 114, 114,
          57, 45, 45,
          48, 39, 39,
          50, 41, 41,
          54, 43, 43,
          60, 78, 78,
          21, 34, 34,
          93, 102, 99,
          112, 74, 74,
          91, 91, 91,
          3, 3, 3,
          1, 1, 1,
          2, 2, 2,
          0, 0, 0,
          79, 79, 79,
          123, 16, 16,
          135, 39, 39,
          117, 35, 35,
          80, 80, 80,
          119, 46, 46,
          62, 40, 40,
          17, 44, 44,
          141, 42, 42,
          113, 9, 77,
          136, 36, 36,
          115, 7, 76,
          134, 153, 153,
          152, 152, 152 };

    }
}
