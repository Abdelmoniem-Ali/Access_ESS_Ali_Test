import { usePidgetIdentity, usePidgetSettings } from '@workspace/utils-react';
import { useQuery } from 'react-query';
import { PidgetSettings } from '../PidgetSettings';

type User = {
    id: string;
    emailAddress: string;
    firstName: string;
    lastName: string;
};

type Users = {
    totalCount: number;
    items: User[];
};

type Organisation = {
    id: string;
    name: string;
    users: Users;
};

type Data = {
    organisation: Organisation;
};

export type Member = {
    data: Data;
};

async function getMember(
    url: string,
    userId: string,
    organisationId: string,
    getAccessToken: () => Promise<string>,
) {
    const token = await getAccessToken();

    const response = await fetch(url, {
        method: 'POST',
        headers: { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' },
        body: JSON.stringify({
            query: `query GetUser($organisationId: ID!, $userId: [ID]) {
                organisation(organisationId: $organisationId) {
                    id
                    name
                    users(query: {take: 100, skip: 0, includeIds: $userId}) {
                        totalCount
                        items {
                            id
                            emailAddress
                            firstName
                            lastName
                            office
                            department
                            jobTitle
                            avatar {
                                id
                                sizes {
                                    size
                                    url
                                }
                            }
                        }
                    }
                }
            }`,
            variables: {
                organisationId,
                userId,
                tagTypesQuery: {
                    includeStatuses: ['ACTIVE'],
                    type: 'TAGS',
                },
            },
        }),
    });
    if (!response.ok) {
        throw Error('Problem fetching character');
    }
    const member = await response.json();
    console.log(member);
    return member;
}

export function useMember() {
    const { getAccessToken, user } = usePidgetIdentity();
    const pidgetSettings = usePidgetSettings<PidgetSettings>();
    return useQuery<Member, Error>(['member', { id: user.id }], () =>
        getMember(pidgetSettings.workspaceApiUrl, user.id, user.organisation.id, getAccessToken),
    );
}
