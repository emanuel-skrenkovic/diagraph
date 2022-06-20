type UseQuerySimpleDefinition = {
    data?: any | undefined,
    isLoading: boolean,
    isError: boolean,
    error?: any | undefined
};

export const useQueryLoading = (query: UseQuerySimpleDefinition, onData?: (data: any) => void): boolean => {
    const { data, isLoading, isError, error } = query;

    if      (isLoading)      return true;
    else if (data && onData) onData(data);
    else if (isError)        console.error(error);

    return false;
};