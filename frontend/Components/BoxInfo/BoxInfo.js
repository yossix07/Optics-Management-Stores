import React, { useState } from "react";
import { View, Text, TextInput, StyleSheet } from "react-native";
import Icon from "@Components/Icon/Icon";
import boxInfoStyles from "./BoxInfoStyles";
import { isFunction } from "@Utilities/Methods";

const BoxInfo = ({ fields, styles }) => {
    const [fieldValues, setFieldValues] = useState(fields);
    
    // update the field values when the fields prop changes
    const changeValues = (index, newText) => {
      const updatedField = { ...fieldValues[index], text: newText };
      const updatedFields = [...fieldValues];
      updatedFields[index] = updatedField;
      setFieldValues(updatedFields);
    };

    const handleEdit = (field) => {
      if(isFunction(field.editFunction)) {
        field.editFunction(fieldValues);
      }
    };

    const handleRemove = (field) => {
      if(isFunction(field.removeFunction)) {
        field.removeFunction(fields);
      }
    };

    const boxStyles = boxInfoStyles();
    return (
        <View style={boxStyles.boxInfo}>
          { fieldValues.map((field, index) => {
            if (field.editable) {
              return (
                <View style={ StyleSheet.compose(boxStyles.boxInfoFunctionalField, styles) } key={ index }>
                  <View style={ boxStyles.textWrapper }>
                    <View style={ boxStyles.textWrapper }>
                      { 
                        field.icon && 
                          <Icon style={ boxStyles.boxInfoIcon } title={ field.icon }/>
                      }
                      <Text style={ boxStyles.text }>
                        { field.label }
                      </Text>
                    </View>
                    <TextInput
                      style={ boxStyles.text }
                      value={ field.text }
                      onChangeText={(text) => changeValues(index, text)}
                    />
                  </View>
                  <Icon title="pen" style={ boxStyles.iconButton } onPress={() => handleEdit(field)} />
                  { field?.removable && (
                    <Icon title="trash" style={ boxStyles.iconButton } onPress={() => handleRemove(field)} />
                  )}
                </View>
              );
            } else if (field.text) {
              return (
                <View 
                  key={ index }
                  style={ field.removable ? StyleSheet.compose(boxStyles.boxInfoFunctionalField, styles)
                  : StyleSheet.compose(boxStyles.boxInfoField, styles) }
                >
                  <View style={ boxStyles.textWrapper }>
                    { field.icon && <Icon style={boxStyles.boxInfoIcon} title={ field.icon } />}
                    <Text style={ boxStyles.text }>
                      { field.text }
                    </Text>
                  </View>
                  { 
                    field?.removable && 
                      <Icon title="trash" style={ boxStyles.iconButton } onPress={() => handleRemove(field)} />
                  }
                </View>
              );
            }
          })}
        </View>
    );
};

export default BoxInfo;