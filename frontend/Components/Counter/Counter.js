import React, { useState, useEffect } from "react";
import { View, Text, StyleSheet } from "react-native";
import Icon from "@Components/Icon/Icon";
import CounterStyles from "./CounterStyles";
import { isFunction, isNumber } from "@Utilities/Methods";

const MIN_VALUE = 1;
const JUMP_VALUE = 1;

const Counter = ({ initValue, minValue=MIN_VALUE, maxValue, onValueChange, style, onAdd, onSubtract }) => {
    const [value, setValue] = useState(MIN_VALUE);

    useEffect(() => {
        if(isNumber(initValue)) {
            setValue(initValue);
        }
    },[]);

    useEffect(() => {
        if(isFunction(onValueChange)) {
            onValueChange(value);
        }
    },[value]);

    const subtract = () => {
        setValue(value => value - JUMP_VALUE);
        
        if(isFunction(onSubtract)) {
            onSubtract(value);
        }
    };

    const handleSubtract = () => {
        if(isNumber(minValue)) {
            if(value > minValue) {
                subtract();
            }
        } else {
            subtract();
        }
    };

    const add = () => {
        setValue(value => value + JUMP_VALUE);
    
        if(isFunction(onAdd)) {
            onAdd(value);
        }
    };

    const handleAdd = () => {
        if(isNumber(maxValue)) {
            if(value < maxValue) {
                add();
            }
        } else {
            add();
        }
    };

    const counterStyles = CounterStyles();

    return (
        <View style={ StyleSheet.compose(counterStyles, style) }>
            <Icon title="subtract" onPress={ handleSubtract } style={ counterStyles.subtractIcon }/>
            <Text style={ counterStyles.counterValue }>
                { value }
            </Text>
            <Icon title="add" onPress={ handleAdd } style={ counterStyles.addIcon }/>
        </View>
    );
};

export default Counter;