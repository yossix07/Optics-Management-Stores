import React, { useEffect, useState, useContext, useCallback } from "react";
import { useFocusEffect } from "@react-navigation/native";
import { ScrollView } from "react-native";
import { api } from "@Services/API";
import GlobalStyles from "@Utilities/Styles";
import { translate } from "@Utilities/translate";
import Cards from "@Components/Cards/Cards";
import { UserContext } from "@Contexts/UserContext";
import BoxInfo from "@Components/BoxInfo/BoxInfo";
import Toast from 'react-native-toast-message';
import { useLoader } from "@Hooks/UseLoader";
import { daysOfWeek, daysOfWeekToIndex } from "@Utilities/Constants";
import { useModal } from "@Hooks/UseModal";
import { isObject } from "@Utilities/Methods";
import { ERROR, SUCCESS, TEXT, LABEL } from "@Utilities/Constants";

const API_CALLS = 4;

const AppointmentsSettingsScreen = ({ navigation }) => {
    const { token } = useContext(UserContext);
    const [appointmentsTypes, setAppointmentsTypes] = useState([]);
    const [daysOff, setDaysOff] = useState([]);
    const [avaliableBlocks, setAvaliableBlocks] = useState([]);
    const [slotDuration, setSlotDuration] = useState([]);
    const [toBeRemoved, setToBeRemoved] = useState({});
    const [isRemovedSccessfully, setIsRemovedSccessfully] = useState(false);
    const [ dataLoaderCount, setDataLoaderCount ] = useState(0);
    const { showLoader, hideLoader } = useLoader();
    const { showModal, hideModal } = useModal();
    const globalStyles = GlobalStyles();

    useFocusEffect(
        useCallback(() => {
        showLoader();
        setDataLoaderCount(0);
        getAppointmentsTypes();
        getDaysOff();
        getAvaliableBlocks();
        getSlotDuration();
        }, [])
    );

    useEffect(() => {
        if(dataLoaderCount === API_CALLS) {
            hideLoader();
        }
    }, [dataLoaderCount]);

    useEffect(() => {
        if(isRemovedSccessfully && isObject(toBeRemoved) && Object.keys(toBeRemoved).length > 0) {
            toBeRemoved.setValues(prev => prev.filter((item) => !toBeRemoved?.isEqual(item, toBeRemoved.removed)));
            setIsRemovedSccessfully(false);
            setToBeRemoved({});
        }
    },[toBeRemoved, isRemovedSccessfully]);

    const mapFieldsToObject = (arr) => {
        const obj = {};
        arr.forEach(item => {
          if (item.hasOwnProperty(TEXT)) {
            if(item.hasOwnProperty(LABEL)) {
                const key = item.label.toLowerCase().split(': ')[0];
                obj[key] = item.text;
            } else {
                const key = item.text.toLowerCase().split(': ')[0];
                obj[key] = item.text.split(': ')[1];
            }
          }
        });
        return obj;
    };

    const succsessGetAppointmentsTypes = (appointmentsTypes) => {
        setDataLoaderCount(prev => prev + 1);
        setAppointmentsTypes(appointmentsTypes);
    };

    const getAppointmentsTypes = () => {
        api?.getAppointmentsTypes(
            token,
            succsessGetAppointmentsTypes,
            handleError
        );
    };

    const successGetAvailableBlocks = (avaliableBlocks) => {
        setDataLoaderCount(prev => prev + 1);
        setAvaliableBlocks(avaliableBlocks);
    };

    const getAvaliableBlocks = () => {
        api?.getAvaliableBlocks(
            token,
            successGetAvailableBlocks,
            handleError
        );
    };

    const successGetDaysOff = (daysOff) => {
        setDataLoaderCount(prev => prev + 1);
        setDaysOff(daysOff);
    };

    const getDaysOff = () => {
        api?.getDaysOff(
            token,
            successGetDaysOff,
            handleError
        );
    };

    const successGetSlotDuration = (duration) => {
        setDataLoaderCount(prev => prev + 1);
        setSlotDuration([duration]);
    };

    const getSlotDuration = () => {
        api?.getSlotDuration(
            token,
            successGetSlotDuration,
            handleError
        );
    };

    const addAppointmentsType = () => {
        navigation.navigate('Add-Appointment-Type');
    };

    const addDayOff = () => {
        navigation.navigate('Add-Day-Off');

    };
    const addAvaliableBlock = () => {
        navigation.navigate('Add-Appointment-Block');
    };

    const addCustomSlot = () => {
        navigation.navigate('Add-Custom-Appointment-Slot');
    };

    const removeAppointmentsType = (item) => {
        const remove = () => {
            showLoader();
            const args = mapFieldsToObject(item);
            args.typeName = args.name;
            delete args.name;
            setToBeRemoved({
                removed: args,
                setValues: setAppointmentsTypes,
                isEqual: isTypesEqual,
            });
            api?.removeAppointmentsType(
                args.typeName,
                token,
                handleSuccessRemove,
                handleError
            );
        }
        
        showModal(
            translate["remove_appointment_type_message"],
            remove,
            hideModal
        );
    };

    const removeDayOff = (item) => {
        const remove = () => {
            showLoader();
            const fields = mapFieldsToObject(item);
            const [year, month, day] = fields.date.split("-");
            const args = {
                year: parseInt(year),
                month: parseInt(month),
                day: parseInt(day),
                date: `${year.trim()}-${month.trim()}-${day.trim()}`
            };
            setToBeRemoved({
                removed: args,
                setValues: setDaysOff,
                isEqual: isDaysOffEqual,
            });
            api?.removeDayOff(
                args,
                token,
                handleSuccessRemove,
                handleError
            );
        }

        showModal(
            translate["remove_day_off_message"],
            remove,
            hideModal
        );
    };
    
    const removeAvaliableBlock = (item) => {
        const remove = () => {
            showLoader();
            const fields = mapFieldsToObject(item);
            const { "start time": startTime, "end time": endTime, "day of week": dayOfWeek } = fields;
            const [startHour, startMinute] = startTime.split(":").map((timePart) => timePart.replace(/^0{0,1}(.+)/, "$1"));
            const [endHour, endMinute] = endTime.split(":").map((timePart) => timePart.replace(/^0{0,1}(.+)/, "$1"));
            const args = { 
                "startHour": parseInt(startHour),
                "startMinute": parseInt(startMinute),
                "endHour": parseInt(endHour),
                "endMinute": parseInt(endMinute),
                "weekday": parseInt(daysOfWeekToIndex[dayOfWeek])
            };
            setToBeRemoved({
                removed: {
                    startTime: startTime,
                    endTime: endTime,
                    dayOfWeek: args.weekday
                },
                setValues: setAvaliableBlocks,
                isEqual: isAvaliableBlocksEqual,
            });
            api?.removeAvaliableBlock(
                args,
                token,
                handleSuccessRemove,
                handleError
            );
        };

        showModal(
            translate["remove_avaliable_block_message"],
            remove,
            hideModal
        );
    };

    const editAppointmentsType = (item) => {
        const edit = () => {
            showLoader();
            const args = mapFieldsToObject(item);
            args.typeName = args.name;
            delete args.name;
            
            api?.editAppointmentsType(
                args,
                token,
                handleSuccessEdit,
                handleError
            );
        };
        
        showModal(
            translate["edit_appointment_type_message"],
            edit,
            hideModal
        );
    };
        
    const editSlotDuration = (item) => {
        const edit = () => {
            showLoader();
            const args = mapFieldsToObject(item);
            api?.editSlotDuration(
                args,
                token,
                handleSuccessEdit,
                handleError
            );
        };

        showModal(
            translate["edit_slot_duration_message"],
            edit,
            hideModal
        );
    };

    const handleSuccessEdit = () => {
        hideLoader();
        Toast.show({
            type: SUCCESS,
            text1: translate["action_success"],
        });
    };

    const handleError = (error) => {
        hideLoader();
        setToBeRemoved({});
        setIsRemovedSccessfully(false);
        Toast.show({
            type: ERROR,
            text1: translate["something_went_wrong"],
            text2: error,
          });
    };

    const handleSuccessRemove = () => {
        hideLoader();
        Toast.show({
            type: SUCCESS,
            text1: translate["action_success"],
        });
        setIsRemovedSccessfully(true);
    };

    const isTypesEqual = (type1, type2) => {
        return type1.typeName === type2.typeName;
    };

    const isAvaliableBlocksEqual = (block1, block2) => {
        return block1.startTime.trim() === block2.startTime.trim() && 
               block1.endTime.trim() === block2.endTime.trim() &&
               block1.dayOfWeek === block2.dayOfWeek;
    };

    const isDaysOffEqual = (day1, day2) => {
        return day1.date == day2.date;
    };

    const cardsData = [
        {
            title: translate["appoinments_types_title"],
            icon: "list",
            list: appointmentsTypes,
            titleButtons: [{ text: "+", onPress: addAppointmentsType }],
            renderItem: (item) => 
                <BoxInfo key={item.typeName} fields={[
                    { text: `${translate["name_placeholder"]}: ${item.typeName}`, removable: true, removeFunction: removeAppointmentsType },
                    { label: `${translate["price_placeholder"]}: `, text: `${item.price}`, editable: true, editFunction: editAppointmentsType }
                ]}/>,
        },
        {
            title: translate["working_hours_title"],
            icon: "calendar",
            list: avaliableBlocks,
            titleButtons: [
                { text: translate["add_custom_slot"], onPress: addCustomSlot },
                { text: "+", onPress: addAvaliableBlock }
            ],
            renderItem: (item) => 
                <BoxInfo key={`${item.startTime}-${item.endTime}-${item.dayOfWeek}`} fields={[
                    { text: `${translate["start_time_placeholder"]} ${item.startTime}`, removable: true, removeFunction: removeAvaliableBlock },
                    { text: `${translate["end_time_placeholder"]} ${item.endTime}` },
                    { text: `${translate["day_of_week_placeholder"]}: ${daysOfWeek[item.dayOfWeek]}` }
                ]}/>,
        },
        {
            title: translate["days_off_title"],
            icon: "xCalendar",
            list: daysOff,
            titleButtons: [{ text: "+", onPress: addDayOff }],
            renderItem: (item) => 
                <BoxInfo key={ item.date } fields={[
                    { text: `${translate["name_placeholder"]} ${item.name}`, removable: true, removeFunction: removeDayOff },
                    { text: `${translate["date_placeholder"]} ${item.date}` },
                ]}/>,
        },
        {
            title: translate["appoinments_duration_title"],
            icon: "clock",
            list: slotDuration,
            renderItem: (item) => 
                <BoxInfo key={ `${item.hours}:${item.minutes}` } fields={[
                    { 
                        label: `${translate["hours_placeholder"]}`,
                        text: `${item.hours}`,
                        editable: true,
                        editFunction: editSlotDuration 
                    },
                    { 
                        label: `${translate["minutes_placeholder"]}`,
                        text: `${item.minutes}`,
                        editable: true,
                        editFunction: editSlotDuration 
                    },
                ]}/>,
        },
    ];

    return(
        <ScrollView style={ globalStyles.container }>
            <Cards cards={ cardsData } />
        </ScrollView>
    );
};

export default AppointmentsSettingsScreen;