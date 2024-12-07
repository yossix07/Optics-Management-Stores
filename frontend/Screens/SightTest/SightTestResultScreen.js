import React from "react";
import { useRoute } from "@react-navigation/native";
import { View } from 'react-native';
import GlobalStyles from "@Utilities/Styles";
import { translate } from '@Utilities/translate';
import SightTestStyles from './SightTestStyles';
import ResultCard from "./ResultCard";
import PressableButton from "@Components/PressableButton/PressableButton";

const PERFECT_VISION = 850;
const ALMOST_PERFECT_VISION = 750;
const GOOD_VISION = 650;
const NORMAL_VISION = 550;
const BAD_VISION = 450; 
const MIN_SCORE = 0;
const MAX_SCORE = 950;

const SightTestResultScreen = ({ navigation }) => {
    const route = useRoute();
    const score = route?.params?.score;
    const globalStyles = GlobalStyles();
    const styles = SightTestStyles();
    
    const getVisionRank = (score) => {
        if(score > PERFECT_VISION) {
            return translate['you_have_perfect_vision'];
        } else if(score > ALMOST_PERFECT_VISION) {
            return translate['you_have_almost_perfect_vision'];
        } else if(score > GOOD_VISION) {
            return translate['might_be_recommended_to_come_by_for_a_checkup'];
        } else if(score > NORMAL_VISION) {
            return translate['it_is_recommended_to_come_by_for_a_checkup'];
        } else if(score > BAD_VISION) {
            return translate['you_should_use_glasses_please_come_by_for_a_checkup'];
        } else {
            return translate['You_defently_should_use_glasses_please_come_by_for_a_checkup'];
        }
    };

    const goToTestScreen = () => {
        navigation.navigate('Sight-Test');
    };
    
    const leftEyeRank = getVisionRank(score?.right);
    const rightEyeRank = getVisionRank(score?.left);
    
    return (
        <View style={ [globalStyles.container, styles.center] }>
            <ResultCard 
                title={ translate['right_eye_result'] }
                icon="rightCircle"
                eyeRank={ rightEyeRank }
                minValue={ MIN_SCORE }
                maxValue={ MAX_SCORE }
                score={ score?.left }
            />
            <ResultCard
                title={ translate['left_eye_result'] }
                icon="leftCircle"
                eyeRank={ leftEyeRank }
                minValue={ MIN_SCORE }
                maxValue={ MAX_SCORE }
                score={ score?.right }
            />
            <PressableButton onPressFunction={ goToTestScreen }>
                { translate['try_again']}
            </PressableButton>
        </View>
    );
};

export default SightTestResultScreen;