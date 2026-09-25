using System;

public class Class1
{
	public Class1()
	{

#if false
 
 
   Sub Plot_SOTOC()

        Days = (End_Date - Start_Date).Days

        If Chart1.ChartAreas.IndexOf("Area1") = -1 Then
            Chart1.ChartAreas.Add("Area1")
        End If

        If Chart1.Series.IndexOf("SO") = -1 Then ' Add closing series if it isn't already plotted
            Chart1.Series.Add("SO")
        End If

        With Chart1.Series("SO")
            .Points.Clear()
            .ChartType = SeriesChartType.Line
            .ChartArea = "Area1"
            .BorderWidth = 2
            .Color = Color.Blue
        End With

        Dim result = Plot_Stochastic(lst_c1high, lst_c1low, lst_c1Closing, 14)

        If Days <= result.Count Then
            Days = result.Count
        End If

        For i As Integer = 0 To result.Count - 1
            Chart1.Series("SO").Points.AddXY(i, result(i))
            c1yValues.Add(result(i))
        Next

        If ChBuy = True Then
            Plot_BuySellIndicators("Area1", c1yValues)
        End If
        '
        Chart1.ChartAreas("Area1").AxisY.Title = Company1
        Chart1.ChartAreas("Area1").AxisX.Title = "Stochastic Oscillator." & vbCrLf & "Showing " & Days & " Days"
    
        End If

    End Sub

     Sub Plot_ROC()

        Days = (End_Date - Start_Date).Days

        If Chart1.ChartAreas.IndexOf("Area1") = -1 Then
            Chart1.ChartAreas.Add("Area1")
        End If

        If Chart1.Series.IndexOf("ROC") = -1 Then
            Chart1.Series.Add("ROC")
        End If

        With Chart1.Series("ROC")
            .ChartArea = "Area1"
            .ChartType = SeriesChartType.Line
            .Color = Color.Brown
            .BorderWidth = 2
        End With

        Dim result = ROC(lst_c1Closing, 14)

        If Days <= result.Count Then
            Days = result.Count
        End If

        For i As Integer = 0 To result.Count - 1
            Chart1.Series("ROC").Points.AddXY(i, result(i))
            c1yValues.Add(result(i))
        Next

        If ChBuy = True Then
            Plot_BuySellIndicators("Area1", c1yValues)
        End If
        Chart1.ChartAreas("Area1").AxisY.Title = Company1
        '
        Chart1.ChartAreas("Area1").AxisX.Title = "Rate of Change" & vbCrLf & "Showing " & Days & " Days"
     

    End Sub



     Sub Plot_EMA()

        Days = (End_Date - Start_Date).Days

        If Chart1.ChartAreas.IndexOf("Area1") = -1 Then
            Chart1.ChartAreas.Add("Area1")
        End If

        If Chart1.Series.IndexOf("EMA") = -1 Then
            Chart1.Series.Add("EMA")
        End If
        With Chart1.Series("EMA")
            .ChartArea = "Area1"
            .ChartType = SeriesChartType.Line
            .BorderWidth = 2
            .Color = Color.Blue
        End With
        Dim result = EMA(lst_c1Closing, 14)
        If Days <= result.Count Then
            Days = result.Count
        End If

        For i As Integer = 0 To result.Count - 1
            Chart1.Series("EMA").Points.AddXY(i, result(i))
        Next

        If ChBuy = True Then
            Plot_BuySellIndicators("Area1", result)
        End If
        Chart1.ChartAreas("Area1").AxisY.Title = Company1
        '
        Chart1.ChartAreas("Area1").AxisX.Title = "Exponential Moving Average." & vbCrLf & "Showing " & Days & " Days"
    

    End Sub


     Sub Plot_CCI()

        Days = (End_Date - Start_Date).Days

        If Chart1.ChartAreas.IndexOf("Area1") = -1 Then
            Chart1.ChartAreas.Add("Area1")
        End If

        If Chart1.Series.IndexOf("CCI") = -1 Then
            Chart1.Series.Add("CCI")
        End If
        With Chart1.Series("CCI")
            .ChartArea = "Area1"
            .ChartType = SeriesChartType.Line
            .BorderWidth = 2
            .Color = Color.Blue
        End With

        Dim result = CCI(lst_c1high, lst_c1low, lst_c1Closing, 14)

        If Days <= result.Count Then
            Days = result.Count
        End If

        For i As Integer = 0 To result.Count - 1
            Chart1.Series("CCI").Points.AddXY(i, result(i))
            c1yValues.Add(result(i))
        Next

        If ChBuy = True Then
            Plot_BuySellIndicators("Area1", c1yValues)
        End If
        Chart1.ChartAreas("Area1").AxisY.Title = Company1
        '
        Chart1.ChartAreas("Area1").AxisX.Title = "Commodity Change Index." & vbCrLf & "Showing " & Days & " Days"
    

    End Sub



   

  



      Public Function Plot_PSAR()

        Days = (End_Date - Start_Date).Days

        If Chart1.ChartAreas.IndexOf("Area1") = -1 Then
            Chart1.ChartAreas.Add("Area1")
        End If

        Dim StartAF As Single = 0.02
        Dim stepAF As Single = 0.02
        Dim maxAF As Single = 0.2
        Dim highPrices As List(Of Single)
        Dim lowPrices As List(Of Single)
        Dim PsarData() As Single

        highPrices = lst_c1high
        lowPrices = lst_c1low
        Chart1.ChartAreas("Area1").AxisY.Title = Company1

        Try
            If Chart1.Series.IndexOf("c1_PSAR") = -1 Then ' Add closing series if it isn't already plotted
                Chart1.Series.Add("c1_PSAR")
            End If

            With Chart1.Series("c1_PSAR")
                .ChartArea = "Area1"
                .ChartType = SeriesChartType.Line
                .BorderWidth = 2
                .Color = Color.Black
            End With
            Chart1.Series("c1_PSAR").Points.Clear()

            If Chart1.Series.IndexOf("c1sPSAR") > 0 Then
                Chart1.Series.Remove(Chart1.Series("c1sPSAR"))
            End If

            Dim c1sPSAR As New Series("c1sPSAR")
            Chart1.Series.Add(c1sPSAR)
            Chart1.Series("c1sPSAR").YAxisType = AxisType.Primary
            Chart1.Series("c1sPSAR").ChartArea = "Area1"
            Chart1.Series("c1sPSAR").Color = ColourBlue
            c1sPSAR.ChartType = SeriesChartType.Point
            c1sPSAR.MarkerStyle = MarkerStyle.Circle
            c1sPSAR.MarkerSize = 6

            ' ======Call the Technical Indicator ============
            PsarData = PSAR(highPrices, lowPrices, StartAF, stepAF, maxAF)
            If Days <= PsarData.Count Then
                Days = PsarData.Count
            End If

            '=============Plot=================
            For i As Integer = 0 To PsarData.Count - 1
                Chart1.Series("c1_PSAR").Points.AddXY(i, lst_c1Closing(i))
                c1sPSAR.Points.AddXY(i, PsarData(i))
                c1yValues.Add(PsarData(i))
            Next

            If ChBuy = True Then
                Plot_BuySellIndicators("Area1", c1yValues)
            End If

            Chart1.ChartAreas("Area1").AxisY.Title = Company1
            '
            Chart1.ChartAreas("Area1").AxisX.Title = "Parabolic Stop and Reverse." & vbCrLf & "Showing " & Days & " Days"


        Catch ex As Exception
            MsgBox(ex.Message, vbOKOnly, "Error Plotting PSAR")
        End Try

      

            Catch ex As Exception
                MsgBox(ex.Message, vbOKOnly, "i= " & i)
            End Try
        End If

        nrSeries += 1

    End Function


//=======================================


    public void PlotPSAR(SfCartesianChart chart, string company)
    {
        int days = (End_Date - Start_Date).Days;

        try
        {
            // Clear the existing chart.
            chart.Series.Clear();

            // Calculate PSAR.
            decimal startAF = 0.02f;
            decimal stepAF = 0.02f;
            decimal maxAF = 0.2f;

            var psarData = PSAR(
                lst_c1high,
                lst_c1low,
                startAF,
                stepAF,
                maxAF);

            if (days <= psarData.Count)
                days = psarData.Count;

            // Store PSAR values for Buy/Sell indicators.
            c1yValues.Clear();

            // -------------------------------------------------
            // Closing price data
            // -------------------------------------------------

            var closingData = new List<ChartPoint>();

            for (int i = 0; i < psarData.Count; i++)
            {
                closingData.Add(new ChartPoint
                {
                    X = i,
                    Y = lst_c1Closing[i]
                });
            }

            // -------------------------------------------------
            // PSAR data
            // -------------------------------------------------

            var psarPoints = new List<ChartPoint>();

            for (int i = 0; i < psarData.Count; i++)
            {
                psarPoints.Add(new ChartPoint
                {
                    X = i,
                    Y = psarData[i]
                });

                c1yValues.Add(psarData[i]);
            }

            // -------------------------------------------------
            // Closing price line
            // -------------------------------------------------

            var closingSeries = new LineSeries
            {
                Label = "Closing Price",
                ItemsSource = closingData,
                XBindingPath = "X",
                YBindingPath = "Y",
                StrokeWidth = 2
            };

            chart.Series.Add(closingSeries);

            // -------------------------------------------------
            // PSAR dots
            // -------------------------------------------------

            var psarSeries = new ScatterSeries
            {
                Label = "PSAR",
                ItemsSource = psarPoints,
                XBindingPath = "X",
                YBindingPath = "Y",
                PointWidth = 6,
                PointHeight = 6,
                Type = ShapeType.Circle
            };

            chart.Series.Add(psarSeries);

            // -------------------------------------------------
            // Axis titles
            // -------------------------------------------------

            if (chart.YAxes.Count > 0)
            {
                chart.YAxes[0].Title = new ChartAxisTitle
                {
                    Text = company
                };
            }

            if (chart.XAxes.Count > 0)
            {
                chart.XAxes[0].Title = new ChartAxisTitle
                {
                    Text = $"Parabolic Stop and Reverse.\nShowing {days} Days"
                };
            }

            // -------------------------------------------------
            // Buy / Sell indicators
            // -------------------------------------------------

            if (ChBuy)
            {
                // Plot_BuySellIndicators will be converted separately.
            }
        }
        catch (Exception ex)
        {
            // Replace with your ShareTrader message box if required.
            System.Diagnostics.Debug.WriteLine(
                $"Error Plotting PSAR: {ex.Message}");
        }
    }



    public void PlotMACD(SfCartesianChart chart, string company)
    {
        int days = (End_Date - Start_Date).Days;

        // Remove anything currently displayed.
        chart.Series.Clear();

        // Calculate MACD.
        var macdResult = MACD(
            lst_c1Closing,
            12,
            26,
            9);

        if (days <= lst_c1Closing.Count)
        {
            days = lst_c1Closing.Count;
        }

        // ---------------------------------------------
        // MACD line
        // ---------------------------------------------

        var macdData = new List<ChartPoint>();

        for (int i = 0; i < macdResult.Count; i++)
        {
            macdData.Add(new ChartPoint
            {
                X = i,
                Y = macdResult[i]
            });
        }

        var macdSeries = new LineSeries
        {
            Label = "MACD",
            ItemsSource = macdData,
            XBindingPath = "X",
            YBindingPath = "Y",
            StrokeWidth = 2
        };

        chart.Series.Add(macdSeries);

        // ---------------------------------------------
        // Axis titles
        // ---------------------------------------------

        if (chart.YAxes.Count > 0)
        {
            chart.YAxes[0].Title = new ChartAxisTitle
            {
                Text = company
            };
        }

        if (chart.XAxes.Count > 0)
        {
            chart.XAxes[0].Title = new ChartAxisTitle
            {
                Text =
                    $"Moving Average Convergence/Divergence.\nShowing {days} Days"
            };
        }
    }

    public void PlotMACD(SfCartesianChart chart, string company)
    {
        int days = (End_Date - Start_Date).Days;

        try
        {
            // Clear the existing chart.
            chart.Series.Clear();

            // MACD settings.
            int fast = 12;
            int slow = 26;
            int signal = 9;

            // Calculate the two EMAs.
            var emaFast = CalculateEMA(lst_c1Closing, fast);
            var emaSlow = CalculateEMA(lst_c1Closing, slow);

            // Calculate MACD values.
            var macdValues = new List<decimal>();

            for (int i = 0; i < lst_c1Closing.Count; i++)
            {
                if (emaFast[i] == decimal.MinValue ||
                    emaSlow[i] == decimal.MinValue)
                {
                    macdValues.Add(decimal.MinValue);
                }
                else
                {
                    macdValues.Add(emaFast[i] - emaSlow[i]);
                }
            }

            // Calculate signal line.
            var signalValues = CalculateEMA(macdValues, signal);

            // Data for MACD line.
            var macdData = new List<MacdPoint>();

            // Data for signal line.
            var signalData = new List<MacdPoint>();

            // Data for histogram.
            var histogramData = new List<MacdPoint>();

            for (int i = 0; i < lst_c1Closing.Count; i++)
            {
                if (macdValues[i] == decimal.MinValue)
                    continue;

                macdData.Add(new MacdPoint
                {
                    X = i,
                    Y = macdValues[i]
                });

                if (signalValues[i] != decimal.MinValue)
                {
                    signalData.Add(new MacdPoint
                    {
                        X = i,
                        Y = signalValues[i]
                    });

                    decimal histogramValue =
                        macdValues[i] - signalValues[i];

                    histogramData.Add(new MacdPoint
                    {
                        X = i,
                        Y = histogramValue,
                        Color = histogramValue >= 0
                            ? Colors.Green
                            : Colors.Red
                    });
                }
            }

            // ---------------------------------------------
            // MACD line
            // ---------------------------------------------

            var macdSeries = new LineSeries
            {
                Label = "MACD",
                ItemsSource = macdData,
                XBindingPath = "X",
                YBindingPath = "Y",
                StrokeWidth = 2
            };

            // ---------------------------------------------
            // Signal line
            // ---------------------------------------------

            var signalSeries = new LineSeries
            {
                Label = "Signal",
                ItemsSource = signalData,
                XBindingPath = "X",
                YBindingPath = "Y",
                StrokeWidth = 1
            };

            // ---------------------------------------------
            // Histogram
            // ---------------------------------------------

            var histogramSeries = new ColumnSeries
            {
                Label = "Histogram",
                ItemsSource = histogramData,
                XBindingPath = "X",
                YBindingPath = "Y",
                PointColorPath = "Color"
            };

            chart.Series.Add(macdSeries);
            chart.Series.Add(signalSeries);
            chart.Series.Add(histogramSeries);

            // ---------------------------------------------
            // Legend
            // ---------------------------------------------

            chart.Legend = new ChartLegend
            {
                IsVisible = true
            };

            // ---------------------------------------------
            // Axis titles
            // ---------------------------------------------

            if (chart.YAxes.Count > 0)
            {
                chart.YAxes[0].Title = new ChartAxisTitle
                {
                    Text = company
                };
            }

            if (chart.XAxes.Count > 0)
            {
                chart.XAxes[0].Title = new ChartAxisTitle
                {
                    Text =
                        $"Moving Average Convergence/Divergence.\nShowing {days} Days"
                };
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Error Plotting MACD: {ex.Message}");
        }
    }



   


   


#endif





    }
}
