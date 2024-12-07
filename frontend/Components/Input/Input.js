import React from 'react';
import { View, TextInput } from 'react-native';
import { useColors } from '@Hooks/UseColors';
import InputStyles from './InputStyles';
import { BIG } from '@Utilities/Constants';

const TEXT_ALIGN = 'center';

const Input = ({ placeholder, onChangeText, value, secureTextEntry, size=BIG }) => {
  const COLORS = useColors();
  const handleChangeText = (text) => {
    onChangeText(text);
  };

  const inputStyle = InputStyles(size);

  return (
    <View style={ inputStyle.inputView }>
      <TextInput
        style={ inputStyle.TextInput }
        placeholder={ placeholder }
        placeholderTextColor={ COLORS.primary_opposite }
        secureTextEntry={ secureTextEntry }
        value= { value }
        onChangeText={ handleChangeText }
        textAlign={ TEXT_ALIGN }
      />
    </View>
  );
};

export default Input;