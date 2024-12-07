import React from "react";
import { View } from "react-native";
import MyText from "@Components/MyText/MyText";
import Card from "@Components/Card/Card";
import ScoreBar from "@Components/ScoreBar/ScoreBar";
import { translate } from "@Utilities/translate";
import SightTestStyles from "./SightTestStyles";

const ResultCard = ({ title, icon, eyeRank, minValue, maxValue, score }) => {
    const styles = SightTestStyles();
    return (
        <Card 
            title={ title }
            icon={ icon}
        >
            <View>
                <MyText style={ styles.resultText }>{ eyeRank }</MyText>
                <View style={ styles.scoreBarWrapper }>
                    <MyText>{ translate["eye_score"] }</MyText>
                    <ScoreBar
                        style={ styles.scoreBar }
                        minValue={ minValue }
                        maxValue={ maxValue }
                        score={ score }
                    />
                </View>
            </View>
        </Card>
    );
};

export default ResultCard;