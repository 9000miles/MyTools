using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace MarsPC
{
    public enum EAnimLayerName
    {
        BaseLayer,
        FullBody,
        LeftArm,
        RightArm,
        OnlyArms,
        UnderBody,
        UpperBody,
        UnderBody_Root,
        UpperBody_Root,
        HeadArm,
    }

    public static class ComponentHelper
    {
        public static void SetLayerWeight(this Animator animator, EAnimLayerName layerName, float value)
        {
            int index = animator.GetLayerIndex(layerName.ToString());
            animator.SetLayerWeight(index, value);
        }

        public static void SetLayerWeight(this Animator animator, string layerName, float value)
        {
            int index = animator.GetLayerIndex(layerName);
            animator.SetLayerWeight(index, value);
        }

        public static int GetLayerIndex(this Animator animator, EAnimLayerName layerName)
        {
            return animator.GetLayerIndex(layerName.ToString());
        }

        public static float GetLayerWeight(this Animator animator, EAnimLayerName layerName)
        {
            int index = animator.GetLayerIndex(layerName.ToString());
            return animator.GetLayerWeight(index);
        }

        public static float GetLayerWeight(this Animator animator, string layerName)
        {
            int index = animator.GetLayerIndex(layerName);
            return animator.GetLayerWeight(index);
        }

        public static Color HexToRGBA(string hex)
        {
            int hexInt = Int32.Parse(hex, NumberStyles.AllowHexSpecifier);
            float r = ((float)((hexInt & 0xFF0000) >> 16)) / 255.0f;
            float g = ((float)((hexInt & 0x00FF00) >> 8)) / 255.0f;
            float b = ((float)((hexInt & 0x0000FF) >> 0)) / 255.0f;
            float a = 1.0f;
            return new Color(r, g, b, a);
        }

        public static void ResetAnimation(this Animation animation, string name)
        {
            animation[name].time = 0;
            animation.Play();
            animation.Sample();
            animation.Stop();
        }
    }
}