import React, { useState, useEffect, useContext } from "react";
import { View, ScrollView } from "react-native";
import GlobalStyles from "@Utilities/Styles";
import { translate } from "@Utilities/translate";
import { api } from "@Services/API";
import StatsScreenStyles from "./StatsScreenStyles";
import Toast from 'react-native-toast-message';
import BarChart from "@Components/BarChart/BarChart";
import { UserContext } from "@Contexts/UserContext";
import Card from "@Components/Card/Card";
import { ERROR } from "@Utilities/Constants";
import GeneralCard from "./GeneralCard";
import MyDatePicker, { RANGE_MODE } from '@Components/MyDatePicker/MyDatePicker';
import PressableButton from "@Components/PressableButton/PressableButton";
import { useLoader } from "@Hooks/UseLoader";

const API_CALLS = 5;

const StatsScreen = () => {
    const { token } = useContext(UserContext);
    const [startDate, setStartDate] = useState({
        day: "",
        month: "",
        year: ""
    });
    const [endDate, setEndDate] = useState({
        day: "",
        month: "",
        year: ""
    });
    const [generalStats, setGeneralStats] = useState({});
    const [productAmount, setProductAmount] = useState({});
    const [productIncome, setProductIncome] = useState({});
    const [ordersAmount, setOrdersAmount] = useState({});
    const [ordersIncome, setOrdersIncome] = useState({});
    const [showDatePickerRange, setShowDatePickerRange] = useState(false);
    const [dataLoaderCount, setDataLoaderCount] = useState(0);
    const { showLoader, hideLoader } = useLoader();

    const graphsData = [
        {
            title: translate["product_units_sold"],
            titleIcon: "box",
            labels: productAmount?.labels,
            values: productAmount?.values
        },
        {
            title: translate["product_total_income"],
            titleIcon: "coins",
            labels: productIncome?.labels,
            values: productIncome?.values
        },
        {
            title: translate["orders_amount"],
            titleIcon: "box",
            labels: ordersAmount?.labels,
            values: ordersAmount?.values
        },
        {
            title: translate["orders_income"],
            titleIcon: "coins",
            labels: ordersIncome?.labels,
            values: ordersIncome?.values
        }
    ]

    useEffect(() => {
        if (startDate.day && startDate.month && startDate?.year &&
            endDate.day && endDate.month && endDate.year) {
            showLoader();
            api?.getGeneralStats(startDate, endDate, token, onGetGeneralStatsSuccess, handleError);
            api?.getProductAmount(startDate, endDate, token, onGetSoldUnitsSuccess, handleError);
            api?.getProductsIncome(startDate, endDate, token, onGetProductsIncomeSuccess, handleError);
            api?.getOrdersAmount(startDate, endDate, token, onGetOrdersAmountSuccess, handleError);
            api?.getOrdersIncome(startDate, endDate, token, onGetOrdersIncomeSuccess, handleError);
        }
    },[startDate, endDate]);

    useEffect(() => {
        if (dataLoaderCount === API_CALLS) {
            setDataLoaderCount(0);
            hideLoader();
        }
    },[dataLoaderCount]);

    const getLabelsAndValues = (data) => {
        const labels = [];
        const values = [];
        Object.keys(data)?.map((key) => {
            labels.push(key);
            values.push(data[key]);
        });

        return {
            labels: labels,
            values: values
        }
    };

    const onConfirmRange = (output) => {
        setShowDatePickerRange(false)
        const [ startYear, startMonth, startDay ] = output.startDateString.split('-')
        const [ endYear, endMonth, endDay ] = output.endDateString.split('-')
        setStartDate({
            day: startDay,
            month: startMonth,
            year: startYear
        });
        setEndDate({
            day: endDay,
            month: endMonth,
            year: endYear
        });
    }

    const openDatePickerRange = () => setShowDatePickerRange(true);

    const onCancelRange = () => setShowDatePickerRange(false);

    const onGetSoldUnitsSuccess = (data) => {
        setProductAmount(getLabelsAndValues(data));
        setDataLoaderCount(prev => prev + 1);
    }

    const onGetProductsIncomeSuccess = (data) => {
        setProductIncome(getLabelsAndValues(data))
        setDataLoaderCount(prev => prev + 1);
    };

    const onGetOrdersAmountSuccess = (data) => {
        setOrdersAmount(getLabelsAndValues(data))
        setDataLoaderCount(prev => prev + 1);
    };

    const onGetOrdersIncomeSuccess = (data) => {
        setOrdersIncome(getLabelsAndValues(data))
        setDataLoaderCount(prev => prev + 1);
    };

    onGetGeneralStatsSuccess = (data) => {
        setGeneralStats(data);
        setDataLoaderCount(prev => prev + 1);
    };

    const handleError = (error) => {
        setDataLoaderCount(prev => prev + 1);
        Toast.show({
            type: ERROR,
            text1: translate["something_went_wrong"],
            text2: error
        });
    };

    const globalStyles = GlobalStyles();
    const styles = StatsScreenStyles();

    return (
        <View style={ globalStyles.container }>
            <MyDatePicker
                isVisible={ showDatePickerRange }
                mode={ RANGE_MODE }
                onCancel={ onCancelRange }
                onConfirm={ onConfirmRange }
            />
            <ScrollView>
                <View style={ styles.header }>
                    <PressableButton onPressFunction={ openDatePickerRange } icon="calendar">
                        {
                            startDate?.day && startDate?.month && startDate?.year && endDate?.day && endDate?.month && endDate?.year ?
                            `${startDate?.day}/${startDate?.month}/${startDate?.year} -- ${endDate?.day}/${endDate?.month}/${endDate?.year}` :
                            translate["select_dates"]
                        }

                    </PressableButton>
                </View>
                {
                    generalStats && Object.keys(generalStats).length > 0 &&
                    <View key={`${startDate.day}-${startDate.month}-${startDate.year}-
                                ${endDate.day}-${endDate.month}-${endDate.year}`}>
                        <GeneralCard generalInfo={ generalStats }/>
                    </View>
                }
                {
                    graphsData.map((graphData, index) => {
                        if (!graphData.labels || !graphData.values) {
                            return null;
                        }
                        return (
                            <Card 
                                key={ index }
                                title={ graphData?.title }
                                icon={ graphData?.titleIcon }
                                fitContent
                            >
                                <BarChart
                                    labels={ graphData.labels }
                                    data={ graphData.values }
                                />
                            </Card>
                        );
                    })
                }
            </ScrollView>
        </View>
    );
};

export default StatsScreen;