import React from "react";
import BoxInfo from "@Components/BoxInfo/BoxInfo";
import { translate } from "@Utilities/translate";

const ProductInfo = ({ product }) => {
    return(
        <BoxInfo fields={[
            {
                icon: 'glasses',
                text: translate['name_placeholder'] + ': ' + product.name,
            },
            {
                icon: 'money',
                text: translate['price_placeholder'] + ': ' + product.price,
            },
            {
                icon: 'list',
                text: translate['quantity_placeholder'] + ': ' + product.quantity,
            },
        ]}/>
    );
};

export default ProductInfo;