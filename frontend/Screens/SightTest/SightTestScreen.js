import React, { useState, useCallback } from 'react';
import { useFocusEffect } from '@react-navigation/core';
import { View } from 'react-native';
import MyText from '@Components/MyText/MyText';
import GlobalStyles from "@Utilities/Styles";
import PressableButton from "@Components/PressableButton/PressableButton";
import { translate } from '@Utilities/translate';
import TestSection from './TestSection';
import SightTestStyles from './SightTestStyles';

const SightTestScreen = ({ navigation }) => {
    const [isTestStarted, setIsTestStarted] = useState(false);
    const globalStyles = GlobalStyles();
    const styles = SightTestStyles();

    useFocusEffect(
        useCallback(() => {
            return () => setTimeout(() => setIsTestStarted(false), 500);
        }, [])
    );

    return(
        <View style={ [globalStyles.container, styles.center, { justifyContent: 'space-between' }] }>
            {
                isTestStarted ?
                    <TestSection
                        navigation={ navigation }
                    /> 
                :
                <View style={ styles.openingScreen }>
                    <Instructions/>
                    <PressableButton
                        onPressFunction={ () => setIsTestStarted(true) }
                        style={ styles.startButton }
                    >
                        { translate['start_test'] }
                    </PressableButton>
                </View>

            }
        </View>
    );
};

export default SightTestScreen;

const Instructions = () => {
    const instructions = [
        translate['welcome_to_our_online_sight_test'],
        translate['you_will_be_shown_a_circle'],
        translate['you_will_select_the_corrcet_circle'],
        translate['you_can_click_i_cant_see'],
        translate['note_timed_test'],
        translate['distance_instruction'],
    ];

    const styles = SightTestStyles();
        
    return(
        <View style={ styles.infoSection }>
            {
                instructions.map((instruction, index) =>
                    <MyText key={ index } style={ styles.instructions }>
                        { instruction }
                    </MyText>
                )
            }
        </View>
    );
};