import React, { useEffect, useRef } from 'react';
import { View, Animated, StyleSheet } from 'react-native';
import ScoreBarStyles from './ScoreBarStyles';

const ANIMATION_DURATION = 1500;

const ScoreBar = ({ style, minValue, maxValue, score, animationDuration=ANIMATION_DURATION }) => {
    const fillAnimation = useRef(new Animated.Value(0)).current;
    const styles = ScoreBarStyles();
    
    useEffect(() => {   
        Animated.timing(fillAnimation, { 
          toValue: score,  
          duration: animationDuration,  
          useNativeDriver: false,   
      }).start();  
    }, [score]);

    const fillPercentage = fillAnimation.interpolate({
      inputRange: [minValue, maxValue],
      outputRange: ['0%', '100%'],
      extrapolate: 'clamp',
    });

    return (
        <View style={ StyleSheet.compose(styles.container, style) }>
            <View style={ styles.barContainer }>
                <Animated.View style={ [styles.fill, { width: fillPercentage }] } />
            </View>
        </View>
    );
};

export default ScoreBar;
