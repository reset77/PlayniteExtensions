﻿using System;
using System.ComponentModel;
using System.Windows;

namespace GamersGateLibrary;

public class SelectorBehaviors
{
    public class EnumSelector
    {
        public Enum Value { get; set; }
        public string DisplayText { get; set; }
    }

    private static readonly DependencyProperty EnumSourceProperty =
        DependencyProperty.RegisterAttached(
        "EnumSource",
        typeof(Type),
        typeof(SelectorBehaviors),
        new PropertyMetadata(new PropertyChangedCallback(EnumSourcePropertyChanged)));

    public static Type GetEnumSource(DependencyObject obj)
    {
        return (Type)obj.GetValue(EnumSourceProperty);
    }

    public static void SetEnumSource(DependencyObject obj, Type value)
    {
        obj.SetValue(EnumSourceProperty, value);
    }

    private static void EnumSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        if (DesignerProperties.GetIsInDesignMode(obj))
        {
            return;
        }

        var select = (System.Windows.Controls.Primitives.Selector)obj;
        if (args.NewValue == null)
        {
            return;
        }

        select.DisplayMemberPath = nameof(EnumSelector.DisplayText);
        select.SelectedValuePath = nameof(EnumSelector.Value);
        select.Items.Clear();
        var vals = Enum.GetValues((Type)args.NewValue);
        foreach (Enum val in vals)
        {
            select.Items.Add(new EnumSelector { Value = val, DisplayText = val.GetDescription() });
        }
    }
}
