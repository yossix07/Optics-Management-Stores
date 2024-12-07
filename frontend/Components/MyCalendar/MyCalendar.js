import React, { useState } from 'react';
import { View } from 'react-native';
import { Calendar, DefaultTheme } from 'react-native-calendars';
import MyCalendarStyles from './MyCalendarStyles';
import { useColors } from '@Hooks/UseColors';
import { isFunction } from "@Utilities/Methods";

const MyCalendar = ({ style, onDayPress, onMonthChange, minDate, hideExtraDays }) => {
    const [selectedDay, setSelectedDay] = useState();
    const styles = MyCalendarStyles();
    const COLORS = useColors();

    const handleDayPress = (day) => {
        setSelectedDay(day?.dateString);
        if (isFunction(onDayPress)) {
            onDayPress(day);
        }
    };

    return(
        <View key={`${COLORS.primary}-${COLORS.secondary}-${COLORS.main}`}>
            <Calendar
                style={ style }
                onDayPress={ handleDayPress }
                minDate={ minDate }
                hideExtraDays={ hideExtraDays }
                onMonthChange={ onMonthChange }
                markedDates={{
                    [selectedDay]: { selected: true, marked: true, disableTouchEvent: true }
                  }}
                theme = {{
                    ...DefaultTheme,
                    ...styles.calendarTheme
                }}
            />
        </View>
    );
};

export default MyCalendar;