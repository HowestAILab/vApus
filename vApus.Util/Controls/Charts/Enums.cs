/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
namespace vApus.Util
{
    public enum AutoScrollXAxis
    {
        KeepAtEnd = 0,
        None,
        KeepAtBeginning
    }
    public enum ChartViewState
    {
        Collapsed = 0,
        Expanded
    }
    public enum LegendViewState
    {
        Collapsed = 0,
        Expanded
    }
    public enum YAxisViewState
    { 
        OnlyShowSelected = 0,
        ShowSelectedAndSeriesWithEqualXAxisValues
    }
}
