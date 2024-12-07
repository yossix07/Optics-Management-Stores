import React from 'react';
import { View, Text, ScrollView, TouchableOpacity, Pressable, StyleSheet } from 'react-native';
import cardStyles from './CardStyles';
import Icon from "@Components/Icon/Icon";
import { isString } from '@Utilities/Methods';

// card component for displaying information
const Card = ({ style, title, icon, titleButtons, children, small, fitContent=false }) => {
  const styles = cardStyles(small, fitContent);
  return (
    <View key={ title } style={ StyleSheet.compose(styles.cardContainer, style) }>
      <View style={ styles.TitleRow }>
        <View style={ styles.IconTitleWrapper }>
          { 
            icon && isString(icon) ? 
            <Icon style={ styles.icon } title={ icon }/> : 
            icon 
          }
          <Text style={ styles.Title }> { title } </Text>
        </View>
        { titleButtons &&
            titleButtons.map((titleButton, index) => 
            <TouchableOpacity key={ index } onPress={ titleButton.onPress }>
              <Pressable onPress={ titleButton.onPress }>
                <Text style={ styles.titleButton }> 
                  { titleButton.text }
                </Text>
              </Pressable>
            </TouchableOpacity>
            )
        }
      </View> 
      <ScrollView showsVerticalScrollIndicator={ true } nestedScrollEnabled = { true }>
        { children }
      </ScrollView>
    </View>
  );
};

export default Card;
