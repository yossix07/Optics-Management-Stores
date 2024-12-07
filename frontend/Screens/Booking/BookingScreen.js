import React, { useState, useEffect, useContext } from 'react';
import { View, FlatList } from 'react-native';
import GlobalStyles from '@Utilities/Styles';
import MyCalendar from '@Components/MyCalendar/MyCalendar';
import { api } from '@Services/API';
import AppointmentSlot from '@Components/AppointmentSlot/AppointmentSlot';
import { translate } from '@Utilities/translate';
import { UserContext } from "@Contexts/UserContext";
import { getCurrentDate, getCurrentMonthLastDate, getMonthLastDate } from '@Utilities/Date';
import BookedAppointment from '@Components/BookedAppointment/BookedAppointment';
import BookingScreenStyles from './BookingScreenStyles';
import MyDropDown from "@Components/MyDropDown/MyDropDown";
import Toast from 'react-native-toast-message';
import { useLoader } from "@Hooks/UseLoader";
import { useModal } from "@Hooks/UseModal";
import Icon from "@Components/Icon/Icon";
import { useFocusEffect } from '@react-navigation/native';
import { useCallback } from 'react';
import { ERROR, SUCCESS } from '@Utilities/Constants';

const AVAILABLE = 0;
const BOOKED = 1;

const BookingScreen = () => {
    const { username, token, isUser, isTenant } = useContext(UserContext); 
    const [currentMonthAppointments, setCurrentMonthAppointments] = useState([]);
    const [currentDate, setCurrentDate] = useState(getCurrentDate());
    const [selectedDate, setSelectedDate] = useState(null);
    const [availableSlots, setAvailableSlots] = useState([]);
    const [selectedSlot, setSelectedSlot] = useState(null);
    const [appointmentsTypes, setAppointmentsTypes] = useState([]);
    const [selectedAppointmentType, setSelectedAppointmentType] = useState(null);
    const [selectedAppointmentStatus, setSelectedAppointmentStatus] = useState(BOOKED);
    const [toBeRemoved, setToBeRemoved] = useState('');

    const appointmentStatuses = [
        { label: translate["available"], value: AVAILABLE },
        { label: translate["booked"], value: BOOKED },
    ];
    const { showLoader, hideLoader } = useLoader();
    const { showModal, hideModal } = useModal();
    const lastDayOfMonth = getCurrentMonthLastDate();

    useFocusEffect(
        useCallback(() => {
            if(isUser()) {
                initUser(currentDate, lastDayOfMonth);
            } else if(isTenant()){
                setSelectedAppointmentStatus(BOOKED);
                getBookedAppointments(currentDate, lastDayOfMonth);
            }
        },[])

    );

    useEffect(() => {
        if (currentMonthAppointments) {
            setAvailableSlots(currentMonthAppointments[selectedDate]);
        }
    },[selectedDate]);

    useEffect(() => {
        if(isTenant()) {
            setAvailableSlots([]);
            const lastDayOfMonth = getMonthLastDate(currentDate.month);
            lastDayOfMonth.year = parseInt(currentDate.year);
            setTenantMonthAppointments(currentDate, lastDayOfMonth);
        }
    },[selectedAppointmentStatus]);

    useEffect(() => {
        if(selectedDate) {
            setAvailableSlots(currentMonthAppointments[selectedDate]);
        }
    },[currentMonthAppointments]);

    useEffect(() => {
        if(toBeRemoved && toBeRemoved !== '') {
          showModal(
            translate["remove_appointment_message"],
            removeAppointment,
            () => {
                setToBeRemoved('');
                hideModal();
            },
          );
        }
      },[toBeRemoved]);

    const removeAppointment = () => {
        showLoader();
        api?.removeAppointment(toBeRemoved.id, username, token, handleSuccessfulAppointmentRemove, handleError);
    };

    const setTenantMonthAppointments = (startDate, endDate) => {
        if(selectedAppointmentStatus === AVAILABLE) {
            getAvailableAppointments(startDate, endDate);
        } else if(selectedAppointmentStatus === BOOKED) {
            getBookedAppointments(startDate, endDate);
        }
    };

    const getAvailableAppointments = (startDate, endDate) => {
        api?.getAvailableAppointments(
            startDate,
            endDate,
            token,
            handleAppointmentsData,
            handleError
        );
    };

    const getBookedAppointments = (startDate, endDate) => {
        api?.getBookedAppointments(
            startDate,
            endDate,
            token,
            handleAppointmentsData,
            handleError
        );
    };

    const initUser = (currentDate, lastDayOfMonth) => {
        getAvailableAppointments(currentDate, lastDayOfMonth);
        api?.getAppointmentsTypes(
            token,
            handleAppointmentsTypesData,
            handleError
        );
    };

    const handleAppointmentsTypesData = (appointmentsTypes) => {
        const types = [];
        appointmentsTypes.forEach(type => {
            types.push({ label: `${type.typeName} - ${type.price}`, value: type.typeName });
        });
        setAppointmentsTypes(types);
    };

    const onDayPress = (day) => {
        setSelectedDate(day?.dateString);
    };

    const onMonthChange = (date) => {
        const today = new Date();
        const currentMonth = String(today.getMonth() + 1);
        let day = '01';
        if (date.month == currentMonth) {
            day = String(today.getDate());
        }
        const currentDate = {
            year: parseInt(date.year),
            month: parseInt(date.month),
            day: parseInt(day)
        }
        setCurrentDate(currentDate);
    };

    useEffect(() => {
        handleMonthChange();
    },[currentDate]);

    const handleMonthChange = () => {
        setAvailableSlots([]);
        setSelectedDate(null);
        
        const lastDayOfMonth = getMonthLastDate(currentDate.month);
        lastDayOfMonth.year = parseInt(currentDate.year);

        if(isUser()) {
            getAvailableAppointments(currentDate, lastDayOfMonth);
        } else if(isTenant()){
            setTenantMonthAppointments(currentDate, lastDayOfMonth);
        }
    };

    const handleAppointmentsData = (appointments) => {
        setCurrentMonthAppointments(appointments.value);
    };

    const handleSlotPress = (slot) => {
        if(isUser()) {
            setSelectedSlot(slot);
        }
    };

    useEffect(() => {
        if (selectedSlot) {
            activateModal();
        }
    },[selectedSlot]);

    const handleSlotBookingApprove = () => {
        showLoader();
        api.bookAppointment(
            selectedSlot?.id,
            username,
            selectedAppointmentType,
            "",
            token,
            successBookingAppointment,
            (error) => { 
                setSelectedSlot(null);
                handleError(error);
            }
        );
    };

    const successBookingAppointment = () => {
        hideLoader();
        Toast.show({
            type: SUCCESS,
            text1: translate["action_success"],
        });
        setCurrentMonthAppointments(prev => {
            const updated = prev[selectedDate].filter(appointment => appointment.id !== selectedSlot.id);
            return {
                ...prev,
                [selectedDate]: updated
            }
        });
        setAvailableSlots(prevSlots => prevSlots.filter(prevSlot => prevSlot.id !== selectedSlot.id));
    }
    
    const onCloseModal = () => {
        setSelectedSlot(null);
    };

    const activateModal = () => {
        showModal(
            translate["confirm_booking_message"],
            handleSlotBookingApprove,
            onCloseModal
        );
    }; 

    const handleError = (error) => {
        hideLoader();
        Toast.show({
            type: ERROR,
            text1: translate["something_went_wrong"],
            text2: error
          });
    };

    const handleSlotRemove = (slot) => {
        setToBeRemoved(slot);
    };

    const handleSuccessfulAppointmentRemove = () => {
        hideLoader();
        Toast.show({
          type: SUCCESS,
          text1: translate["action_success"],
        });
        setAvailableSlots(prev => {
          const updated = prev.filter(appointment => appointment.id !== toBeRemoved.id);
          return updated;
        });

        setCurrentMonthAppointments(prev => {
            Object.keys(prev).forEach(date => {
                prev[date] = prev[date].filter(appointment => appointment.id !== toBeRemoved.id);
            });
            return prev;
        });
        setToBeRemoved('');
      };

    const styles = BookingScreenStyles();
    const globalStyles = GlobalStyles();
    
    return (
        <View style={ globalStyles.container }>
            { isUser() && 
                <MyDropDown
                    value={ selectedAppointmentType }
                    items={ appointmentsTypes }
                    setValue={ setSelectedAppointmentType }
                    placeholder={ translate["select_appointment_type"] }
                />
            }
            { isTenant() && 
                <MyDropDown
                    value={ selectedAppointmentStatus }
                    items={ appointmentStatuses }
                    setValue={ setSelectedAppointmentStatus }
                    placeholder={ translate["select_appointment_status"] }
                />
            }
            <View>
                <MyCalendar
                    onDayPress={ onDayPress }
                    minDate={ new Date().toString() }
                    hideExtraDays={ true }
                    onMonthChange={ onMonthChange }
                />
                { selectedDate && 
                    <AppointmentsSlots 
                        styles={ styles }
                        availableSlots={ availableSlots }
                        isUser={ isUser }
                        isTenant={ isTenant }
                        selectedAppointmentStatus={ selectedAppointmentStatus }
                        handleSlotPress={ handleSlotPress }
                        handleSlotRemove={ handleSlotRemove }
                    />
                }
            </View>
        </View>
    );
};

export default BookingScreen;

const AppointmentsSlots = ({
    styles,
    availableSlots,
    isUser,
    isTenant,
    selectedAppointmentStatus,
    handleSlotPress,
    handleSlotRemove
}) => {

    return (
        <View style={ styles.slotsView }>
            <FlatList
                data={ availableSlots }
                renderItem={({ item }) => {
                    if(isUser() || selectedAppointmentStatus === AVAILABLE) {
                        return(
                            <AppointmentSlot
                                item={ item }
                                onPress={ handleSlotPress }
                            />
                        );
                    }
                    else if(isTenant()) {
                        return(
                            <BookedAppointment
                                item={ item }
                                titleButtons={[
                                    { 
                                        text:<Icon title="trash" style={{ color: styles.clickableColor }}/>,
                                        onPress: () => handleSlotRemove(item) }
                                  ]}
                            />
                        );
                    }
                }}
            />
        </View>
    );
}