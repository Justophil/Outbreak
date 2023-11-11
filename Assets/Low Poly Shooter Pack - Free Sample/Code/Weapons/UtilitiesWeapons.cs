// Copyright 2021, Infima Games. All Rights Reserved.

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary>
    /// Weapon Static Utilities.
    /// </summary>
    public static class UtilitiesWeapons
    {
        /// <summary>
        /// Enables one object, disables all others.
        /// </summary>
        public static T SelectAndSetActive<T>(this T[] array, int index) where T : MonoBehaviour
        {
            // Make sure we have objects in the array! If we don't, we could get an error or a crash here.
            if (array == null || array.Length == 0)
                return null;

            // Deactivate All. This way we don't have to do it manually.
            for (int i = 0; i < array.Length; i++)
            {
                array[i]?.gameObject.SetActive(false);
            }

            // Error Check.
            if (index < 0 || index >= array.Length)
                return null;

            // Activate.
            T behaviour = array[index];
            if (behaviour != null)
                behaviour.gameObject.SetActive(true);

            // Return.
            return behaviour;
        }
    }
}