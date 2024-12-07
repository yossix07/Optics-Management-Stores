import React from "react";
import BoxInfo from "@Components/BoxInfo/BoxInfo";
import Card from "@Components/Card/Card";
import { translate } from "@Utilities/translate";
import { PENDING, READY, DELIVERD, CANCELED } from "@Utilities/Constants";
import StatsScreenStyles from "./StatsScreenStyles";

const GeneralCard = ({ generalInfo }) => {
    const styles = StatsScreenStyles();

    const fields = [
        {
            icon: 'cart',
            text: `${translate['products_sold']}: ${generalInfo?.productsAmount}`,
        },
        {
            icon: `${PENDING.toLowerCase()}`,
            text: `${translate['pending_orders']}: ${generalInfo?.pendingOrdersAmount}`,
        },
        {
            icon: `${READY.toLowerCase()}`,
            text: `${translate['ready_orders']}: ${generalInfo?.readyOrdersAmount}`,
        },
        {
            icon: `${DELIVERD.toLowerCase()}`,
            text: `${translate['delivered_orders']}: ${generalInfo?.deliverdOrdersAmount}`,
        },
        {
            icon: `${CANCELED.toLowerCase()}`,
            text: `${translate['canceled_orders']}: ${generalInfo?.canceledOrdersAmount}`,
        },
    ];

    const key = `${generalInfo?.pendingOrdersAmount}-${generalInfo?.readyOrdersAmount}-
                 ${generalInfo?.deliverdOrdersAmount}-${generalInfo?.canceledOrdersAmount}-
                 ${generalInfo?.productsAmount}`;

    return (
        <Card 
            key={ key } 
            title={ translate['general_card_title']}
            icon="info"
        >
            <BoxInfo
                styles={ styles.generalInfo }
                fields={ fields } />
        </Card>
    );

};

export default GeneralCard;