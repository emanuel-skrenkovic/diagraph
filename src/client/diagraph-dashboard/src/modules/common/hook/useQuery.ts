import { useLocation } from 'react-router-dom';

export const useQuery = (paramName: string) => {
    const location = useLocation();
    const query = new URLSearchParams(location.search);
    return query.get(paramName);
};