import React from "react";
import { View } from "react-native";
import Card from "@Components/Card/Card";

const Cards = ({ cards }) => {
    return(
        cards?.map((cardData, index) => 
            <Card
                title={ cardData?.title }
                icon={ cardData?.icon }
                titleButtons={ cardData?.titleButtons }
                key={ index }
            >
                <View>
                    { cardData?.list?.map((item, index) => {
                        return(
                            <View key={ index }>
                                { cardData?.renderItem(item) }
                            </View>
                        )
                    })}
                    { cardData?.dict && Object.keys(cardData.dict).map(key => {
                        return cardData.dict[key].map((item, index) => 
                          <View key={ index }>
                              { cardData?.renderItem([key,item]) }
                          </View>
                          )  
                    }
                    )}
                </View>
            </Card>
        )
    );
};

export default Cards;