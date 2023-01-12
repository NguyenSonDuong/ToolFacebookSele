let joinGroups = (uid) => {
    let body = {
        "feedType": "DISCUSSION",
        "groupID": uid,
        "imageMediaType": "image/x-auto",
        "input": {
            "attribution_id_v2": "CometGroupDiscussionRoot.react,comet.group,unexpected,1666667080043,656387,2361831622,;SearchCometGlobalSearchTopTabRoot.react,comet.search_results.top_tab,tap_search_bar,1666667072721,122815,391724414624676,",
            "group_id": uid,
            "group_share_tracking_params": {
                "app_id": "",
                "exp_id": "null",
                "is_from_share": false
            },
            "source": "group_mall",
            "actor_id": require("CurrentUserInitialData").USER_ID,
            "client_mutation_id": "4"
        },
        "inviteShortLinkKey": null,
        "isChainingRecommendationUnit": false,
        "isEntityMenu": false,
        "scale": 1,
        "source": "GROUP_MALL",
        "renderLocation": "group_mall",
        "__relay_internal__pv__CometGlobalPanelEMCopresencerelayprovider": false,
        "__relay_internal__pv__GroupsCometEntityMenuEmbeddedrelayprovider": false
    };
    let dataBody =
    {
        "av": require("CurrentUserInitialData").USER_ID,
        "__user": require("CurrentUserInitialData").USER_ID,
        "__a": "1",
        "fb_dtsg": require("DTSGInitialData").token,
        "fb_api_caller_class": "RelayModern",
        "fb_api_req_friendly_name": "GroupCometJoinForumMutation",
        "variables": body,
        "server_timestamps": true,
        "doc_id": "5716527145033962"
    };

    let formBody = [];
    for (let property in dataBody) {
        let encodedKey = encodeURIComponent(property);
        if (encodedKey == variables) {
            encodedValue = JSON.stringify(dataBody[property]);
        } else {
            encodedValue = encodeURIComponent(dataBody[property]);
        }

        formBody.push(encodedKey + "=" + encodedValue);
    }
    formBody = formBody.join("&");

    return new Promise(async (resolve, reject) => {
        fetch("https://www.facebook.com/api/graphql/", {
            "headers": {
                "accept": "/",
                "accept-language": "en-US,en;q=0.9",
                "content-type": "application/x-www-form-urlencoded",
                "sec-ch-prefers-color-scheme": "light",
                "sec-ch-ua": "\".Not/A)Brand\";v=\"99\", \"Google Chrome\";v=\"103\", \"Chromium\";v=\"103\"",
                "sec-ch-ua-mobile": "?0",
                "sec-ch-ua-platform": "\"Windows\"", "sec-fetch-dest": "empty", "sec-fetch-mode": "cors", "sec-fetch-site": "same-origin",
                "content-type": "application/x-www-form-urlencoded",
            },
            "referrer": "https://www.facebook.com/groups/" + uid,
            "referrerPolicy": "origin-when-cross-origin",
            "body": formBody,
            "method": "POST",
            "mode": "cors",
            "credentials": "include"
        }).then((data) => { return data })
            .then((data2) => {
                data2.text().then((data) => {
                    try {
                        let obj = JSON.parse(data);
                        resolve(JSON.stringify(obj));
                    } catch (e) {
                        resolve(data);
                    }
                });
            });;

    });
}

return joinGroups("%<ID_GROUPS>%");