﻿using System;
using Xamarin.Forms.Internals;

namespace XF.Material.Outline.iOS
{
    [Preserve(AllMembers = true)]
    public static class Outline
    {
        public static bool IsInitialised = false;

        public static void Init()
        {
            if(!IsInitialised)
            {
                IsInitialised = true;
            }
        }
    }
}
