import React, { useState, useEffect, useRef } from 'react';
import { View, TouchableOpacity } from 'react-native';
import MyText from '@Components/MyText/MyText';
import PressableButton from "@Components/PressableButton/PressableButton";
import HoledCircle from '@Components/HoledCircle/HoledCircle';
import { shuffleArray } from '@Utilities/Array';
import Toast from 'react-native-toast-message';
import { translate } from '@Utilities/translate';
import { useModal } from '@Hooks/UseModal';
import SightTestStyles from './SightTestStyles';
import { SUCCESS } from '@Utilities/Constants';

const circleButtonSize = 120;
const InitCircleSize = 100;
const smallestCircleSize = 30;
const circleSizeOffset = 10;
const InitCircleOpacity = 1;
const smallestCircleOpacity = 0.3;
const opacityBonus = 0.03;
const sizeBonus = 3;
const circleOpacityOffset = 0.1;
const degreeOffset = 45;
const degreeOffsetNumber = 360;
const roundsPerEye = 12;
const MAX_RESPOSNE_TIME = 7000;
const MIN_TIME_SCORE = 0;
const WRONG_ASWER = -1;
const RIGHT_EYE = 'right';
const LEFT_EYE = 'left';
const timeWeight = 0.001;
const sizeWeight = 1;
const opacityWeight = 100;

const TestSection = ({ navigation }) => {
    const [eye, setEye] = useState(RIGHT_EYE);
    const [circleDegree, setCircleDegree] = useState(0);
    const [circleOpacity, setCircleOpacity] = useState(InitCircleOpacity);
    const [circleSize, setCircleSize] = useState(InitCircleSize);
    const [userScore, setUserScore] = useState({ right: 0, left: 0 });
    const [answersDegrees, setAnswersDegrees] = useState([0, 0, 0]);
    const [round, setRound] = useState(1);
    const { showModal } = useModal();
    const startTimeRef = useRef(0);
    const styles = SightTestStyles();

    useEffect(() => {
        generateRandomCircle();
    }, [circleSize, circleOpacity]);

    useEffect(() => {
        if (round >= roundsPerEye) {
            if (eye === RIGHT_EYE) {
                showModal(
                    translate['switch_eyes_message'],
                    handleSwitchEye,
                );
                setEye(LEFT_EYE);
            } else {
                goToResultScreen();
            }
            setRound(1);
            setCircleOpacity(InitCircleOpacity);
            setCircleSize(InitCircleSize);
        }
    },[round]);

    const goToResultScreen = () => {
        Toast.show({
            type: SUCCESS,
            text1: translate['completed_test_message'],
        });
        navigation.navigate('Sight-Test-Result', { 
            score: userScore
        });
    };

    const generateRandomCircle = () => {
        const randomDegree = degreeOffset * Math.floor(Math.random() * degreeOffsetNumber);
        setCircleDegree(randomDegree);
        const answers = [randomDegree, randomDegree + degreeOffset, randomDegree - degreeOffset];
        setAnswersDegrees(shuffleArray(answers));
        startTimeRef.current = new Date().getTime();
    };
    
    const handlePickedAnswer = (degree) => {
        const endTime = new Date().getTime();
        const responseTime = endTime - startTimeRef.current;
        setRound((round) => round + 1);
    
        if (degree === circleDegree) {
          const timeScore = calculateTimeScore(responseTime);
          const sizeScore = calculateSizeScore(circleSize);
          const opacityScore = calculateOpacityScore(circleOpacity);
          const totalScore = timeScore + sizeScore + opacityScore;
          setUserScore((scoreDict) => {
            return {
              ...scoreDict,
              [eye]: scoreDict[eye] + totalScore,
            };
          });
          increaseDifficulty();
        } else {
          decreaseDifficulty();
        }
        generateRandomCircle();
    };

    const calculateTimeScore = (responseTime) => {
        const timeScore = timeWeight * (MAX_RESPOSNE_TIME - responseTime);
        return timeScore > MIN_TIME_SCORE ? timeScore : MIN_TIME_SCORE;
    };

    const calculateSizeScore = (circleSize) => {
        const sizeScore = sizeWeight * (InitCircleSize - circleSize + sizeBonus);
        return sizeScore;
    };
    
    const calculateOpacityScore = (circleOpacity) => {
        const opacityScore = opacityWeight * (1 - circleOpacity + opacityBonus);
        return opacityScore;
    };

    const increaseDifficulty = () => {
        setCircleOpacity((opacity) => Math.max(opacity - circleOpacityOffset, smallestCircleOpacity));
        setCircleSize((size) => Math.max(size - circleSizeOffset, smallestCircleSize));
    };

    const decreaseDifficulty = () => {
        setCircleOpacity((opacity) => Math.min(opacity + circleOpacityOffset, InitCircleOpacity));
        setCircleSize((size) => Math.min(size + circleSizeOffset, InitCircleSize));
    };

    const handleSwitchEye = () => {
        generateRandomCircle();
    };
    return (
        <View style={ styles.testSection }>
            <MyText style={ styles.instructions }>
                { translate["cover_your_eye_instruction"](eye) }
            </MyText>
            <View style={ styles.mainCircleWrapper }>
                <HoledCircle
                    style={{ opacity: circleOpacity }}
                    height={ circleSize }
                    width={ circleSize }
                    degree={ circleDegree }
                />
            </View>
            <MyText style={ styles.instructions }>
                { translate['select_correct_circle'] }
            </MyText>
            <View>
                <View style={ styles.buttonsWrapper }>
                    {
                        answersDegrees.map((degree, index) => {
                            return (
                                <TouchableOpacity onPress={ () => { handlePickedAnswer(degree) } } key={ index }>
                                    <HoledCircle height={ circleButtonSize } width={ circleButtonSize } degree={ degree }/>
                                </TouchableOpacity>
                            );
                        })
                    }
                </View>
                <PressableButton onPressFunction ={ () => { handlePickedAnswer(WRONG_ASWER) }}>
                    { translate['cant_see'] }
                </PressableButton>
            </View>
        </View>
    );
};

export default TestSection;