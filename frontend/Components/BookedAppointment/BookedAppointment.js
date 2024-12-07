import React from "react";
import BoxInfo from "@Components/BoxInfo/BoxInfo";
import { translate } from "@Utilities/translate";
import Card from "@Components/Card/Card";

// component to show booked appointment details to the doctor with the patient name and the appointment time
const BookedAppointment = ({ item, titleButtons }) => {
    return (
        <Card
            icon="clock"
            title={ `${item?.startTime} ${item?.type?.typeName ? '- ' + item?.type?.typeName : ''}` }
            titleButtons={ titleButtons }
            small={ true }
        >
            <BoxInfo fields={[
            {
                icon: 'user',
                text: `${translate['name_placeholder']} : ${item.userName}`,
            },
            {
                icon: 'envelope',
                text: `${translate['email_placeholder']} : ${item.userEmail}`,
            },
            {
                icon: 'phone',
                text: `${translate['phone_placeholder']} : ${item.userPhoneNumber}`,
            },
            ]}/>
        </Card>
    );
};

export default BookedAppointment;