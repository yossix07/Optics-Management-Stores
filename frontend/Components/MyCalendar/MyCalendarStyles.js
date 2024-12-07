import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const MyCalendarStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    calendarTheme: {
        backgroundColor: COLORS.main,
        calendarBackground: COLORS.main,
        textSectionTitleColor: COLORS.main_opposite,
        selectedDayBackgroundColor: COLORS.primary,
        dayTextColor: COLORS.main_opposite,
        textDisabledColor: COLORS.grey,
        arrowColor: COLORS.main_opposite,
        monthTextColor: COLORS.main_opposite,
        indicatorColor: COLORS.main_opposite,
    },
  });
};

export default MyCalendarStyles;