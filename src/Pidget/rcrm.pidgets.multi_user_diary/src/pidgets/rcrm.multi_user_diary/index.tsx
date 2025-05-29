import { Spinner } from '@tag/tag-components-react-v4';
import { PidgetLoadProps } from '@workspace/utils';
import { PidgetContainer } from '@workspace/utils-react';
import React, { lazy, Suspense } from 'react';
import { QueryClient, QueryClientProvider } from 'react-query';
import { MemoryRouter } from 'react-router-dom';
import { createRoot } from 'react-dom/client';

import { PidgetSettings } from './PidgetSettings';

const App = lazy(() => import('./App'));
const queryClient = new QueryClient();

export async function load(props: PidgetLoadProps<PidgetSettings>) {
    const pidgetElem = props.pidget.element.getElementsByClassName('product-widget');
    const rootElement = pidgetElem.length > 0 ? pidgetElem[0] : props.pidget.element;
    const root = createRoot(rootElement);
    root.render(
        <React.StrictMode>
            <PidgetContainer {...props}>
                <QueryClientProvider client={queryClient}>
                    <MemoryRouter>
                        <Suspense
                            fallback={
                                <div
                                    style={{
                                        position: 'absolute',
                                        top: '50%',
                                        left: '50%',
                                        margin: '-26px 0 0 -26px',
                                    }}
                                >
                                    <Spinner>Loading</Spinner>
                                </div>
                            }
                        >
                            <App root={root} />
                        </Suspense>
                    </MemoryRouter>
                </QueryClientProvider>
            </PidgetContainer>
        </React.StrictMode>,
    );
}
