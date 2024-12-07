import React, { useEffect, useState } from "react";
import { ScrollView, Text, View } from "react-native";
import { BarChart as BC } from "react-native-chart-kit";
import BarChartStyles from "./BarChartStyles";
import { Dimensions } from "react-native";
import { useColors } from "@Hooks/UseColors";
import { isDateString, isFirstDayOfNextMonth } from "@Utilities/Date";
import { translate } from "@Utilities/translate";

const { height, width } = Dimensions.get('window');

const CHART_HEIGHT = 0.5 * height;
const CHART_WIDTH = 0.95 * width;
const LABEL_ROTATION_OFFSET = 8;
const LABEL_ROTATION_MAX = 85;

const BarChart = ({ labels, data }) => {
    const styles = BarChartStyles();
    const [barsColor, setBarsColor] = useState([]);
    const [barLabels, setBarLabels] = useState([]);
    const [showPredictionText, setShowPredictionText] = useState(false);
    const COLORS = useColors();

    useEffect(() => {
        setShowPredictionText(false);
        const newLabels = [];

        // If the labels are dates, we change the format to MM-YYYY
        // and change the color of the last bar in case it is of the next month
        if(labels && labels.length > 0 && isDateString(labels[0])) {
            labels.forEach((label) => {
                const [year, month, day] = label.split('-');
                newLabels.push(`${month}-${year}`);
            });

            const lastLabel = labels?.slice(-1)[0];
            if(lastLabel && isDateString(lastLabel) && isFirstDayOfNextMonth(lastLabel)) {
                handleBarsColor(true);
            } else {
                handleBarsColor(false);
            }
        } else {
            newLabels.push(...labels);
            handleBarsColor(false);
        }
        setBarLabels(newLabels);
    }, [labels]);

    // set bars colors. 
    // if received true, the last bar will be colored in a different color
    const handleBarsColor = (isLastDiffrentColor) => {
        const colors = [];
        data.forEach(() => {
            colors.push(() => COLORS.primary);
        });
        if(isLastDiffrentColor) {
            colors.pop();
            colors.push(() => COLORS.secondary);
            setShowPredictionText(true);
        }
        setBarsColor(colors);
    }

    return (
        <View>
            { 
                showPredictionText &&
                <Text style={ styles.predictionText }>
                    { translate['note_next_month_is_a_prediction'] }
                </Text>
            }
            <ScrollView horizontal={ true } contentContainerStyle={ styles.scrollViewContainer }>
                <BC
                    style={ styles.chartStyle }
                    width={ CHART_WIDTH }
                    height={ CHART_HEIGHT }
                    chartConfig={ styles.chartConfig }
                    verticalLabelRotation={
                        labels.length * LABEL_ROTATION_OFFSET < LABEL_ROTATION_MAX ? 
                        labels.length * LABEL_ROTATION_OFFSET : LABEL_ROTATION_MAX 
                    }
                    showValuesOnTopOfBars
                    fromZero
                    withCustomBarColorFromData={ true }
                    data={{
                        labels: barLabels,
                        datasets: [
                            {
                                data: data,
                                colors: barsColor,
                            }
                        ]
                    }}
                    />
            </ScrollView>
        </View>
    );
};

export default BarChart;
