import { StyleSheet } from 'react-native';
import { useColors } from '@Hooks/UseColors';

const MyDatePickerStyles = () => {
  const COLORS = useColors();
  
  return StyleSheet.create({
    calendarTheme: {
        backgroundColor: COLORS.main,
        headerColor: COLORS.primary,
        headerTextColor: COLORS.primary_opposite,
        weekDaysColor: COLORS.main_opposite,
        dateTextColor: COLORS.main_opposite,
        selectedDateTextColor: COLORS.primary_opposite,
        selectedDateBackgroundColor: COLORS.primary,
        confirmButtonColor: COLORS.primary,
        cancelButtonColor: COLORS.primary,
    },
  });
};

export default MyDatePickerStyles;