import React from "react";
import { StyleSheet, Text, TouchableOpacity, Pressable, View } from "react-native";
import PressableButtonStyles from "./PressableButtonStyles";
import Icon from "@Components/Icon/Icon";

const PressableButton = ({ onPressFunction, children, style, icon }) => {

  const touchableOpacityStyle = PressableButtonStyles();
                        
    if(icon) {
      return (
        <TouchableOpacity style={  StyleSheet.compose(touchableOpacityStyle.touchableOpacityStyle, style) } onPress={ onPressFunction }>
          <Pressable onPress={ onPressFunction }>
              <View style={ touchableOpacityStyle.buttonContent }>
                <Icon title={ icon } style={ touchableOpacityStyle.buttonIcon }/>
                <Text style={ touchableOpacityStyle.buttonText }>
                  { children }
                </Text>
              </View>
          </Pressable>
        </TouchableOpacity>
      )
    }

    return (
      <TouchableOpacity style={  StyleSheet.compose(touchableOpacityStyle.touchableOpacityStyle, style) } onPress={ onPressFunction }>
          <Pressable onPress={ onPressFunction }>
              <Text style={ touchableOpacityStyle.buttonText }>
                { children }
              </Text>
          </Pressable>
      </TouchableOpacity>
    )
}

export default PressableButton;