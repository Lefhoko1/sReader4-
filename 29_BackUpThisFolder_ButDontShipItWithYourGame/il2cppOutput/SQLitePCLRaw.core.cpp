#include "pch-cpp.hpp"





struct VirtualActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename T1>
struct VirtualActionInvoker1
{
	typedef void (*Action)(void*, T1, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename T1, typename T2>
struct VirtualActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4>
struct VirtualActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename R>
struct VirtualFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename R, typename T1>
struct VirtualFuncInvoker1
{
	typedef R (*Func)(void*, T1, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename R, typename T1, typename T2>
struct VirtualFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename R, typename T1, typename T2, typename T3, typename T4>
struct VirtualFuncInvoker4
{
	typedef R (*Func)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename R, typename T1, typename T2, typename T3, typename T4, typename T5>
struct VirtualFuncInvoker5
{
	typedef R (*Func)(void*, T1, T2, T3, T4, T5, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, p4, p5, invokeData.method);
	}
};
struct GenericVirtualActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename T1>
struct GenericVirtualActionInvoker1
{
	typedef void (*Action)(void*, T1, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename T1, typename T2>
struct GenericVirtualActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4>
struct GenericVirtualActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename R>
struct GenericVirtualFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename R, typename T1, typename T2>
struct GenericVirtualFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename R, typename T1, typename T2, typename T3, typename T4, typename T5>
struct GenericVirtualFuncInvoker5
{
	typedef R (*Func)(void*, T1, T2, T3, T4, T5, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, p4, p5, invokeData.method);
	}
};
struct InterfaceActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename T1>
struct InterfaceActionInvoker1
{
	typedef void (*Action)(void*, T1, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename T1, typename T2>
struct InterfaceActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3>
struct InterfaceActionInvoker3
{
	typedef void (*Action)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4>
struct InterfaceActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename R>
struct InterfaceFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename R, typename T1>
struct InterfaceFuncInvoker1
{
	typedef R (*Func)(void*, T1, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename R, typename T1, typename T2>
struct InterfaceFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename R, typename T1, typename T2, typename T3>
struct InterfaceFuncInvoker3
{
	typedef R (*Func)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
	}
};
template <typename R, typename T1, typename T2, typename T3, typename T4>
struct InterfaceFuncInvoker4
{
	typedef R (*Func)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename R, typename T1, typename T2, typename T3, typename T4, typename T5>
struct InterfaceFuncInvoker5
{
	typedef R (*Func)(void*, T1, T2, T3, T4, T5, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, p4, p5, invokeData.method);
	}
};
template <typename R, typename T1, typename T2, typename T3, typename T4, typename T5, typename T6>
struct InterfaceFuncInvoker6
{
	typedef R (*Func)(void*, T1, T2, T3, T4, T5, T6, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, p4, p5, p6, invokeData.method);
	}
};
template <typename R, typename T1, typename T2, typename T3, typename T4, typename T5, typename T6, typename T7>
struct InterfaceFuncInvoker7
{
	typedef R (*Func)(void*, T1, T2, T3, T4, T5, T6, T7, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, p4, p5, p6, p7, invokeData.method);
	}
};
template <typename R, typename T1, typename T2, typename T3, typename T4, typename T5, typename T6, typename T7, typename T8, typename T9>
struct InterfaceFuncInvoker9
{
	typedef R (*Func)(void*, T1, T2, T3, T4, T5, T6, T7, T8, T9, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, p4, p5, p6, p7, p8, p9, invokeData.method);
	}
};
struct GenericInterfaceActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename T1>
struct GenericInterfaceActionInvoker1
{
	typedef void (*Action)(void*, T1, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename T1, typename T2>
struct GenericInterfaceActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4>
struct GenericInterfaceActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename R>
struct GenericInterfaceFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename R, typename T1, typename T2>
struct GenericInterfaceFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename R, typename T1, typename T2, typename T3, typename T4, typename T5>
struct GenericInterfaceFuncInvoker5
{
	typedef R (*Func)(void*, T1, T2, T3, T4, T5, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, p4, p5, invokeData.method);
	}
};

struct ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522;
struct ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517;
struct ConcurrentDictionary_2_t211FFBACF3ECB80F94746F5D14A5187C12AC7E96;
struct ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476;
struct ConcurrentDictionary_2_tF598E45B2A3ECB23FD311D829FB0AB32B1201ACF;
struct Dictionary_2_t87EDE08B2E48F793A22DE50D6B3CC2E7EBB2DB54;
struct EqualityComparer_1_tE29394521901AC9A533E45B9990C0246B75F11E0;
struct EqualityComparer_1_t622E93DC712395AAC84C30749F11878E251A358F;
struct EqualityComparer_1_t92563A67F1C1ECDC3FE387C46498E2E56B59F3C2;
struct Func_4_t969FE6B8E28BD4E700C6BA2ED5F8794B88E37083;
struct ICollection_1_t3F13F61A78BE3BD019937B11A562337BB8F30736;
struct ICollection_1_tD7413105CA5DBF6629BE5E9EE453204D7C0D90FB;
struct IEnumerator_1_tAA6EEADDF0E7FE9E72FAC727374287C9627ADF3A;
struct IEqualityComparer_1_tED581F2C423FD1B93E069AFE7AA4483EF32AF8DB;
struct IEqualityComparer_1_t3991A111A74746C3ABFB18C32691649F882EF67F;
struct IEqualityComparer_1_t0C62219A7981BC3254B9E9404B17F934FE7D7908;
struct IEqualityComparer_1_t2CA7720C7ADCCDECD3B02E45878B4478619D5347;
struct Tables_t35D24A3F197F5DF6339E865510D977E346348C2E;
struct Tables_t8C1D30AC376005522595773E928AB9B1BC8510B1;
struct Tables_tF8B692A39CCD1172C63C26932872799C325C1612;
struct KeyValuePair_2U5BU5D_tA4E5920EB77328443FF447F3D9752258408E93EB;
struct KeyValuePair_2U5BU5D_t34EC4B9FDB0AB82E0FBD7603757C05475234B170;
struct KeyValuePair_2U5BU5D_t70BA81F7DD7CDE54D27420EDD10D4DE3E86BBDA1;
struct ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031;
struct CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB;
struct DelegateU5BU5D_tC5AB7E8F745616680F337909D3A8E6C722CDF771;
struct IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832;
struct StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF;
struct StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248;
struct TypeU5BU5D_t97234E1129B564EB38B8D85CAC2AD8B5B9522FFB;
struct sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894;
struct ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263;
struct AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C;
struct Attribute_tFDA8EFEFB0711976D22474794576DAF28F7440AA;
struct Binder_t91BFCE95A7057FADF4D8A1A342AFE52872246235;
struct CodePageDataItem_t52460FA30AE37F4F26ACB81055E58002262F19F2;
struct CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA;
struct CompareFuncName_tDD5FD56A7AAA8A632A4B49F594E21250BFB40795;
struct DecoderFallback_t7324102215E4ED41EC065C02EB501CB0BC23CD90;
struct Delegate_t;
struct DelegateData_t9B286B493293CD2D23A5B2B5EF0E5B1324C2B77E;
struct EmbeddedAttribute_tA4674B43E8DDF11FB95382E2DF287778F940BFBA;
struct EncoderFallback_tD2C40CE114AA9D8E1F7196608B2D088548015293;
struct Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095;
struct EntryPointAttribute_t153F60F730CBAB2CE127F07F09554695F9B9D075;
struct Exception_t;
struct FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A;
struct IAsyncResult_t7B9B5A0ECB35DCEC31B8A8122C37D687369253B5;
struct IDictionary_t6D03155AF1FA9083817AA5B6AD7DEEACC26AB220;
struct IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5;
struct ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7;
struct InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB;
struct IsByRefLikeAttribute_tE3DF4234BFCF4858A0D7C555BB5DC3A332D72370;
struct IsReadOnlyAttribute_tB8CFB53741972E7671A35C8614A6613F318DBB0A;
struct MemberFilter_tF644F1AE82F611B677CE1964D5A3277DDA21D553;
struct MethodInfo_t;
struct MonoPInvokeCallbackAttribute_t17BE665B3D7CD95379096445F10A2E7124970063;
struct PreserveAttribute_t684A021602CCD4E643B8148B5315F6EDDCCD0D12;
struct SafeGCHandle_tE8D8C107E75BFC0FAC19C271622895B407785D3E;
struct SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7;
struct SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6;
struct String_t;
struct Type_t;
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915;
struct authorizer_hook_info_t27B31C9940EEBF2F29DD86AED0B1A0A04487768A;
struct collation_hook_info_tDD9E78A2C2512E1FEF753347B75A4DEC251D9E98;
struct commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78;
struct delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933;
struct delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF;
struct delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A;
struct delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3;
struct delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0;
struct delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957;
struct delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5;
struct delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712;
struct delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779;
struct delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710;
struct delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174;
struct delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172;
struct delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80;
struct exec_hook_info_tB2C6835FA07DBFA56D9C701D6A2C0FC6D9201CA8;
struct function_hook_info_t2884BE4D2AF455A249E6382005E52E62CDB7E017;
struct hook_handle_tADC84A43AFFDADC460E97AB35367B400F2EB2EFC;
struct hook_handles_tC761D4D430F6003B396C0AE3B469BDD80B3D4C33;
struct log_hook_info_tDC91F629B2B139A944265087648F73E1BB31A116;
struct profile_hook_info_tE9263823781920B9D354CC58CF351F325EDF3D34;
struct progress_hook_info_tF0C73E17649E97704B133D3007AD666D3B416641;
struct rollback_hook_info_t413A1090EF5A7F4DC0305C94AAA26DB193291185;
struct sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2;
struct sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30;
struct sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057;
struct sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C;
struct sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475;
struct sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F;
struct sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD;
struct strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29;
struct strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E;
struct strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583;
struct strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1;
struct strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C;
struct strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7;
struct strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886;
struct trace_hook_info_t4C641DC6C4FE4BFDFACF1B300BA0A794A91A53F4;
struct update_hook_info_tC07EBFD924B99F90D87267BC47281F4AC1098968;
struct agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07;
struct scalar_sqlite3_context_t520AE3E6198561DEF6B32657E820E9B8FBDE0509;
struct U3CU3Ec__DisplayClass263_0_t9EA15DBB375982C7857A45FFC12C73F0B4238EB9;
struct U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF;
struct U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F;
struct U3CU3Ec__DisplayClass274_0_t75B1EC8A8B2CD9CFE76F0CDD3507D686F9C95A31;
struct U3CU3Ec__DisplayClass275_0_t2E8E6521F4006ACCD7E5F26205A9BC7CBA6A4B9A;
struct U3CU3Ec__DisplayClass332_0_t1173985050ED9406B43934B3B874C78753CF564A;
struct U3CU3Ec__DisplayClass429_0_tF649DDD522F0D50321341692A00F5965CC3248D6;

IL2CPP_EXTERN_C RuntimeClass* ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* CompareFuncName_tDD5FD56A7AAA8A632A4B49F594E21250BFB40795_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Exception_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEnumerable_1_tB6F68D35F9622A77D895A483A02F4DC2907BBAEB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEnumerator_1_tAA6EEADDF0E7FE9E72FAC727374287C9627ADF3A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IEqualityComparer_1_tED581F2C423FD1B93E069AFE7AA4483EF32AF8DB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass263_0_t9EA15DBB375982C7857A45FFC12C73F0B4238EB9_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass274_0_t75B1EC8A8B2CD9CFE76F0CDD3507D686F9C95A31_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass275_0_t2E8E6521F4006ACCD7E5F26205A9BC7CBA6A4B9A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass332_0_t1173985050ED9406B43934B3B874C78753CF564A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass429_0_tF649DDD522F0D50321341692A00F5965CC3248D6_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* authorizer_hook_info_t27B31C9940EEBF2F29DD86AED0B1A0A04487768A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* collation_hook_info_tDD9E78A2C2512E1FEF753347B75A4DEC251D9E98_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* exec_hook_info_tB2C6835FA07DBFA56D9C701D6A2C0FC6D9201CA8_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* function_hook_info_t2884BE4D2AF455A249E6382005E52E62CDB7E017_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* log_hook_info_tDC91F629B2B139A944265087648F73E1BB31A116_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* profile_hook_info_tE9263823781920B9D354CC58CF351F325EDF3D34_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* progress_hook_info_tF0C73E17649E97704B133D3007AD666D3B416641_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* rollback_hook_info_t413A1090EF5A7F4DC0305C94AAA26DB193291185_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* scalar_sqlite3_context_t520AE3E6198561DEF6B32657E820E9B8FBDE0509_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* trace_hook_info_t4C641DC6C4FE4BFDFACF1B300BA0A794A91A53F4_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* update_hook_info_tC07EBFD924B99F90D87267BC47281F4AC1098968_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral4EBC86E0EACFCA522AEB82874860D0E248D782A5;
IL2CPP_EXTERN_C String_t* _stringLiteral5F8F7F20A301184F38050D40710A390698DAEC1D;
IL2CPP_EXTERN_C String_t* _stringLiteral929AB203C6C048DBA2C6EA10E47D89A2FDE3F41A;
IL2CPP_EXTERN_C String_t* _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709;
IL2CPP_EXTERN_C String_t* _stringLiteralF165B374B6AD91BF7008BDDE41B21D6BC0612DB5;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentDictionary_2_TryRemove_m1782697D740431D81873A6F2B420EBBDD4FED32D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentDictionary_2_TryRemove_m8220F91E42BAB284E34184616737AAB2A4C9FEF6_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentDictionary_2_TryRemove_m8C588451D72CA78D8618557F8A75D35324158B5D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentDictionary_2__ctor_m738B333B82DBBF304A15728C98883F5163204374_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentDictionary_2__ctor_mC0BBBB7D4FBDEA79D4636FBDB872E0AC54E2C776_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentDictionary_2__ctor_mD8C0B49C9BC53C925E6FAB2B37BADF318BAA208B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentDictionary_2_get_Item_mE58842628CBB5BB71F07874570EC30E7FC721445_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentDictionary_2_get_Values_m2C4385FFCDA16FF0FB436BE7E6127E2F473040FC_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentDictionary_2_get_Values_mF2008A5617C18F3F234996E8F3070D5C9F7ABC10_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentDictionary_2_set_Item_m0A8DC29085CC370BCE112BE5175D0766C86B5121_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentDictionary_2_set_Item_m0B68193309F23091AEE40F02FBB000FAFB452DF8_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ConcurrentDictionary_2_set_Item_mB18463C3EF5A1B49665FDDC7A7418F69CA3F69F0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* EqualityComparer_1__ctor_m98FB31E1349FDCF9F0A3BD1891F322A1AC6CFBC6_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* EqualityComparer_1__ctor_mF420C788DC290244A73738F4BC6F567E4B8B065D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ReadOnlySpan_1_GetPinnableReference_m365106BC7655B6A45D47673D473A699B5B69DA57_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ReadOnlySpan_1_Slice_mEB3D3A427170FC5A0AB734619D4792C299697C89_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ReadOnlySpan_1__ctor_m470D1527EF015478E8677C7BCB52C8410A1DB604_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ReadOnlySpan_1_op_Implicit_mCEA7A54A72D5D6EADEFE280B4927119123C8E644_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Span_1_GetPinnableReference_m55DA180AC02A047DAC0626C7B8CBC2E87626DD0C_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Span_1__ctor_mE18EBB601FBFA01BA29FE353364700952A9091FE_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Span_1_op_Implicit_mD249188242C0C9D3192A31E9F7FA74C683F05B84_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CU3Ec__DisplayClass263_0_U3Csqlite3_config_logU3Eb__0_m8B4286DB43D892CCAC433BFC538CE4D0117CC508_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CU3Ec__DisplayClass269_0_U3Csqlite3_traceU3Eb__0_m3D7C6C768B76831B199A3DEEB4508F55616E75EC_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CU3Ec__DisplayClass271_0_U3Csqlite3_profileU3Eb__0_mC0125502FE73FA12ED8241058AA31E17A35BD0C7_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CU3Ec__DisplayClass274_0_U3Csqlite3_update_hookU3Eb__0_m9A7EB3C9B70FD4005DD270F143CDE0D73F91C033_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CU3Ec__DisplayClass275_0_U3Csqlite3_create_collationU3Eb__0_m04DCDC3168B2FE4CB267E97F21757904478F22F4_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CU3Ec__DisplayClass332_0_U3Csqlite3_execU3Eb__0_m09F33029DA46B08016391B82A5E88D8D6CF03F17_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CU3Ec__DisplayClass429_0_U3Csqlite3_set_authorizerU3Eb__0_m134F19636E67D69A3A7841EEA84C02D1E5FCA1D5_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* sqlite3_find_stmt_m6DB56FD791338E8E5524252CE51E90D7D6624FE3_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* utf8z_FromSpan_mCA9D0C027632A3293CD3FBB43AF9887C124D1E44_RuntimeMethod_var;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;

struct ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031;
struct DelegateU5BU5D_tC5AB7E8F745616680F337909D3A8E6C722CDF771;
struct IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832;
struct StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248;
struct sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
struct U3CModuleU3E_tCBD14DE722393F9659B3BDB279FE71652AA529B2 
{
};
struct ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522  : public RuntimeObject
{
	Tables_t35D24A3F197F5DF6339E865510D977E346348C2E* ____tables;
	RuntimeObject* ____comparer;
	bool ____growLockArray;
	int32_t ____budget;
	KeyValuePair_2U5BU5D_tA4E5920EB77328443FF447F3D9752258408E93EB* ____serializationArray;
	int32_t ____serializationConcurrencyLevel;
	int32_t ____serializationCapacity;
};
struct ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517  : public RuntimeObject
{
	Tables_t8C1D30AC376005522595773E928AB9B1BC8510B1* ____tables;
	RuntimeObject* ____comparer;
	bool ____growLockArray;
	int32_t ____budget;
	KeyValuePair_2U5BU5D_t34EC4B9FDB0AB82E0FBD7603757C05475234B170* ____serializationArray;
	int32_t ____serializationConcurrencyLevel;
	int32_t ____serializationCapacity;
};
struct ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476  : public RuntimeObject
{
	Tables_tF8B692A39CCD1172C63C26932872799C325C1612* ____tables;
	RuntimeObject* ____comparer;
	bool ____growLockArray;
	int32_t ____budget;
	KeyValuePair_2U5BU5D_t70BA81F7DD7CDE54D27420EDD10D4DE3E86BBDA1* ____serializationArray;
	int32_t ____serializationConcurrencyLevel;
	int32_t ____serializationCapacity;
};
struct EqualityComparer_1_tE29394521901AC9A533E45B9990C0246B75F11E0  : public RuntimeObject
{
};
struct EqualityComparer_1_t622E93DC712395AAC84C30749F11878E251A358F  : public RuntimeObject
{
};
struct Attribute_tFDA8EFEFB0711976D22474794576DAF28F7440AA  : public RuntimeObject
{
};
struct CriticalFinalizerObject_t1DCAB623CAEA6529A96F5F3EDE3C7048A6E313C9  : public RuntimeObject
{
};
struct Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095  : public RuntimeObject
{
	int32_t ___m_codePage;
	CodePageDataItem_t52460FA30AE37F4F26ACB81055E58002262F19F2* ___dataItem;
	bool ___m_deserializedFromEverett;
	bool ___m_isReadOnly;
	EncoderFallback_tD2C40CE114AA9D8E1F7196608B2D088548015293* ___encoderFallback;
	DecoderFallback_t7324102215E4ED41EC065C02EB501CB0BC23CD90* ___decoderFallback;
};
struct FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A  : public RuntimeObject
{
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___U3CnameU3Ek__BackingField;
	int32_t ___U3CnU3Ek__BackingField;
};
struct MemberInfo_t  : public RuntimeObject
{
};
struct String_t  : public RuntimeObject
{
	int32_t ____stringLength;
	Il2CppChar ____firstChar;
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F  : public RuntimeObject
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_pinvoke
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_com
{
};
struct authorizer_hook_info_t27B31C9940EEBF2F29DD86AED0B1A0A04487768A  : public RuntimeObject
{
	delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* ____func;
	RuntimeObject* ____user_data;
};
struct collation_hook_info_tDD9E78A2C2512E1FEF753347B75A4DEC251D9E98  : public RuntimeObject
{
	delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* ____func;
	RuntimeObject* ____user_data;
};
struct commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78  : public RuntimeObject
{
	delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* ___U3C_funcU3Ek__BackingField;
	RuntimeObject* ___U3C_user_dataU3Ek__BackingField;
};
struct exec_hook_info_tB2C6835FA07DBFA56D9C701D6A2C0FC6D9201CA8  : public RuntimeObject
{
	delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* ____func;
	RuntimeObject* ____user_data;
};
struct function_hook_info_t2884BE4D2AF455A249E6382005E52E62CDB7E017  : public RuntimeObject
{
	delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* ____func_scalar;
	delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* ____func_step;
	delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* ____func_final;
	RuntimeObject* ____user_data;
};
struct hook_handles_tC761D4D430F6003B396C0AE3B469BDD80B3D4C33  : public RuntimeObject
{
	ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522* ___collation;
	ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517* ___scalar;
	ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517* ___agg;
	RuntimeObject* ___update;
	RuntimeObject* ___rollback;
	RuntimeObject* ___commit;
	RuntimeObject* ___trace;
	RuntimeObject* ___profile;
	RuntimeObject* ___progress;
	RuntimeObject* ___authorizer;
};
struct log_hook_info_tDC91F629B2B139A944265087648F73E1BB31A116  : public RuntimeObject
{
	delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* ____func;
	RuntimeObject* ____user_data;
};
struct profile_hook_info_tE9263823781920B9D354CC58CF351F325EDF3D34  : public RuntimeObject
{
	delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* ____func;
	RuntimeObject* ____user_data;
};
struct progress_hook_info_tF0C73E17649E97704B133D3007AD666D3B416641  : public RuntimeObject
{
	delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* ____func;
	RuntimeObject* ____user_data;
};
struct raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C  : public RuntimeObject
{
};
struct rollback_hook_info_t413A1090EF5A7F4DC0305C94AAA26DB193291185  : public RuntimeObject
{
	delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* ____func;
	RuntimeObject* ____user_data;
};
struct trace_hook_info_t4C641DC6C4FE4BFDFACF1B300BA0A794A91A53F4  : public RuntimeObject
{
	delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* ____func;
	RuntimeObject* ____user_data;
};
struct update_hook_info_tC07EBFD924B99F90D87267BC47281F4AC1098968  : public RuntimeObject
{
	delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* ____func;
	RuntimeObject* ____user_data;
};
struct util_tBD9E5F8A2316101B2B01B75A0B74AC4328007C08  : public RuntimeObject
{
};
struct U3CU3Ec__DisplayClass263_0_t9EA15DBB375982C7857A45FFC12C73F0B4238EB9  : public RuntimeObject
{
	strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* ___f;
};
struct U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF  : public RuntimeObject
{
	strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* ___f;
	RuntimeObject* ___v;
};
struct U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F  : public RuntimeObject
{
	strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* ___f;
	RuntimeObject* ___v;
};
struct U3CU3Ec__DisplayClass274_0_t75B1EC8A8B2CD9CFE76F0CDD3507D686F9C95A31  : public RuntimeObject
{
	strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* ___f;
};
struct U3CU3Ec__DisplayClass275_0_t2E8E6521F4006ACCD7E5F26205A9BC7CBA6A4B9A  : public RuntimeObject
{
	strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* ___f;
};
struct U3CU3Ec__DisplayClass332_0_t1173985050ED9406B43934B3B874C78753CF564A  : public RuntimeObject
{
	strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* ___callback;
};
struct U3CU3Ec__DisplayClass429_0_tF649DDD522F0D50321341692A00F5965CC3248D6  : public RuntimeObject
{
	strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* ___f;
};
struct Boolean_t09A6377A54BE2F9E6985A8149F19234FD7DDFE22 
{
	bool ___m_value;
};
struct Byte_t94D9231AC217BE4D2E004C4CD32DF6D099EA41A3 
{
	uint8_t ___m_value;
};
struct Char_t521A6F19B456D956AF452D926C32709DC03D6B17 
{
	Il2CppChar ___m_value;
};
struct CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA  : public EqualityComparer_1_tE29394521901AC9A533E45B9990C0246B75F11E0
{
	Func_4_t969FE6B8E28BD4E700C6BA2ED5F8794B88E37083* ____f;
};
struct CompareFuncName_tDD5FD56A7AAA8A632A4B49F594E21250BFB40795  : public EqualityComparer_1_t622E93DC712395AAC84C30749F11878E251A358F
{
	RuntimeObject* ____ptrlencmp;
};
struct Double_tE150EF3D1D43DEE85D533810AB4C742307EEDE5F 
{
	double ___m_value;
};
struct EmbeddedAttribute_tA4674B43E8DDF11FB95382E2DF287778F940BFBA  : public Attribute_tFDA8EFEFB0711976D22474794576DAF28F7440AA
{
};
struct EntryPointAttribute_t153F60F730CBAB2CE127F07F09554695F9B9D075  : public Attribute_tFDA8EFEFB0711976D22474794576DAF28F7440AA
{
	String_t* ___U3CNameU3Ek__BackingField;
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2  : public ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F
{
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2_marshaled_pinvoke
{
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2_marshaled_com
{
};
struct Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C 
{
	int32_t ___m_value;
};
struct Int64_t092CFB123BE63C28ACDAF65C68F21A526050DBA3 
{
	int64_t ___m_value;
};
struct IntPtr_t 
{
	void* ___m_value;
};
struct IsByRefLikeAttribute_tE3DF4234BFCF4858A0D7C555BB5DC3A332D72370  : public Attribute_tFDA8EFEFB0711976D22474794576DAF28F7440AA
{
};
struct IsReadOnlyAttribute_tB8CFB53741972E7671A35C8614A6613F318DBB0A  : public Attribute_tFDA8EFEFB0711976D22474794576DAF28F7440AA
{
};
struct MonoPInvokeCallbackAttribute_t17BE665B3D7CD95379096445F10A2E7124970063  : public Attribute_tFDA8EFEFB0711976D22474794576DAF28F7440AA
{
};
struct PreserveAttribute_t684A021602CCD4E643B8148B5315F6EDDCCD0D12  : public Attribute_tFDA8EFEFB0711976D22474794576DAF28F7440AA
{
	bool ___AllMembers;
	bool ___Conditional;
};
struct UInt32_t1833D51FFA667B18A5AA4B8D34DE284F8495D29B 
{
	uint32_t ___m_value;
};
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915 
{
	union
	{
		struct
		{
		};
		uint8_t Void_t4861ACF8F4594C3437BB48B6E56783494B843915__padding[1];
	};
};
struct ByReference_1_t9C85BCCAAF8C525B6C06B07E922D8D217BE8D6FC 
{
	intptr_t ____value;
};
struct ByReference_1_t7BA5A6CA164F770BC688F21C5978D368716465F5 
{
	intptr_t ____value;
};
struct Delegate_t  : public RuntimeObject
{
	intptr_t ___method_ptr;
	intptr_t ___invoke_impl;
	RuntimeObject* ___m_target;
	intptr_t ___method;
	intptr_t ___delegate_trampoline;
	intptr_t ___extra_arg;
	intptr_t ___method_code;
	intptr_t ___interp_method;
	intptr_t ___interp_invoke_impl;
	MethodInfo_t* ___method_info;
	MethodInfo_t* ___original_method_info;
	DelegateData_t9B286B493293CD2D23A5B2B5EF0E5B1324C2B77E* ___data;
	bool ___method_is_virtual;
};
struct Delegate_t_marshaled_pinvoke
{
	intptr_t ___method_ptr;
	intptr_t ___invoke_impl;
	Il2CppIUnknown* ___m_target;
	intptr_t ___method;
	intptr_t ___delegate_trampoline;
	intptr_t ___extra_arg;
	intptr_t ___method_code;
	intptr_t ___interp_method;
	intptr_t ___interp_invoke_impl;
	MethodInfo_t* ___method_info;
	MethodInfo_t* ___original_method_info;
	DelegateData_t9B286B493293CD2D23A5B2B5EF0E5B1324C2B77E* ___data;
	int32_t ___method_is_virtual;
};
struct Delegate_t_marshaled_com
{
	intptr_t ___method_ptr;
	intptr_t ___invoke_impl;
	Il2CppIUnknown* ___m_target;
	intptr_t ___method;
	intptr_t ___delegate_trampoline;
	intptr_t ___extra_arg;
	intptr_t ___method_code;
	intptr_t ___interp_method;
	intptr_t ___interp_invoke_impl;
	MethodInfo_t* ___method_info;
	MethodInfo_t* ___original_method_info;
	DelegateData_t9B286B493293CD2D23A5B2B5EF0E5B1324C2B77E* ___data;
	int32_t ___method_is_virtual;
};
struct Exception_t  : public RuntimeObject
{
	String_t* ____className;
	String_t* ____message;
	RuntimeObject* ____data;
	Exception_t* ____innerException;
	String_t* ____helpURL;
	RuntimeObject* ____stackTrace;
	String_t* ____stackTraceString;
	String_t* ____remoteStackTraceString;
	int32_t ____remoteStackIndex;
	RuntimeObject* ____dynamicMethods;
	int32_t ____HResult;
	String_t* ____source;
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager;
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces;
	IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___native_trace_ips;
	int32_t ___caught_in_unmanaged;
};
struct Exception_t_marshaled_pinvoke
{
	char* ____className;
	char* ____message;
	RuntimeObject* ____data;
	Exception_t_marshaled_pinvoke* ____innerException;
	char* ____helpURL;
	Il2CppIUnknown* ____stackTrace;
	char* ____stackTraceString;
	char* ____remoteStackTraceString;
	int32_t ____remoteStackIndex;
	Il2CppIUnknown* ____dynamicMethods;
	int32_t ____HResult;
	char* ____source;
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager;
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces;
	Il2CppSafeArray* ___native_trace_ips;
	int32_t ___caught_in_unmanaged;
};
struct Exception_t_marshaled_com
{
	Il2CppChar* ____className;
	Il2CppChar* ____message;
	RuntimeObject* ____data;
	Exception_t_marshaled_com* ____innerException;
	Il2CppChar* ____helpURL;
	Il2CppIUnknown* ____stackTrace;
	Il2CppChar* ____stackTraceString;
	Il2CppChar* ____remoteStackTraceString;
	int32_t ____remoteStackIndex;
	Il2CppIUnknown* ____dynamicMethods;
	int32_t ____HResult;
	Il2CppChar* ____source;
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager;
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces;
	Il2CppSafeArray* ___native_trace_ips;
	int32_t ___caught_in_unmanaged;
};
struct GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC 
{
	intptr_t ___handle;
};
struct GCHandleType_t4CD45A3495E593D093AB0CE36EF9EC1A1572F82A 
{
	int32_t ___value__;
};
struct RuntimeTypeHandle_t332A452B8B6179E4469B69525D0FE82A88030F7B 
{
	intptr_t ___value;
};
struct SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7  : public CriticalFinalizerObject_t1DCAB623CAEA6529A96F5F3EDE3C7048A6E313C9
{
	intptr_t ___handle;
	int32_t ____state;
	bool ____ownsHandle;
	bool ____fullyInitialized;
};
struct sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C  : public RuntimeObject
{
	intptr_t ____p;
	RuntimeObject* ____user_data;
	RuntimeObject* ___state;
};
struct sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD  : public RuntimeObject
{
	intptr_t ____p;
};
struct RawData_t37CAF2D3F74B7723974ED7CEEE9B297D8FA64ED0  : public RuntimeObject
{
	intptr_t ___Bounds;
	intptr_t ___Count;
	uint8_t ___Data;
};
struct RawData_t37CAF2D3F74B7723974ED7CEEE9B297D8FA64ED0_marshaled_pinvoke
{
	intptr_t ___Bounds;
	intptr_t ___Count;
	uint8_t ___Data;
};
struct RawData_t37CAF2D3F74B7723974ED7CEEE9B297D8FA64ED0_marshaled_com
{
	intptr_t ___Bounds;
	intptr_t ___Count;
	uint8_t ___Data;
};
struct ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D 
{
	ByReference_1_t9C85BCCAAF8C525B6C06B07E922D8D217BE8D6FC ____pointer;
	int32_t ____length;
};
struct ReadOnlySpan_1_t59614EA6E51A945A32B02AB17FBCBDF9A5C419C1 
{
	ByReference_1_t7BA5A6CA164F770BC688F21C5978D368716465F5 ____pointer;
	int32_t ____length;
};
struct Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305 
{
	ByReference_1_t9C85BCCAAF8C525B6C06B07E922D8D217BE8D6FC ____pointer;
	int32_t ____length;
};
struct MulticastDelegate_t  : public Delegate_t
{
	DelegateU5BU5D_tC5AB7E8F745616680F337909D3A8E6C722CDF771* ___delegates;
};
struct MulticastDelegate_t_marshaled_pinvoke : public Delegate_t_marshaled_pinvoke
{
	Delegate_t_marshaled_pinvoke** ___delegates;
};
struct MulticastDelegate_t_marshaled_com : public Delegate_t_marshaled_com
{
	Delegate_t_marshaled_com** ___delegates;
};
struct SafeGCHandle_tE8D8C107E75BFC0FAC19C271622895B407785D3E  : public SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7
{
};
struct SystemException_tCC48D868298F4C0705279823E34B00F4FBDB7295  : public Exception_t
{
};
struct Type_t  : public MemberInfo_t
{
	RuntimeTypeHandle_t332A452B8B6179E4469B69525D0FE82A88030F7B ____impl;
};
struct sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2  : public SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7
{
	ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476* ____stmts;
	RuntimeObject* ___extra;
};
struct sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30  : public SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7
{
};
struct sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057  : public SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7
{
};
struct sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475  : public SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7
{
};
struct sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F  : public SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7
{
	sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ____db;
};
struct agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07  : public sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C
{
};
struct scalar_sqlite3_context_t520AE3E6198561DEF6B32657E820E9B8FBDE0509  : public sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C
{
};
struct Func_4_t969FE6B8E28BD4E700C6BA2ED5F8794B88E37083  : public MulticastDelegate_t
{
};
struct ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263  : public SystemException_tCC48D868298F4C0705279823E34B00F4FBDB7295
{
	String_t* ____paramName;
};
struct AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C  : public MulticastDelegate_t
{
};
struct InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB  : public SystemException_tCC48D868298F4C0705279823E34B00F4FBDB7295
{
};
struct delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF  : public MulticastDelegate_t
{
};
struct delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A  : public MulticastDelegate_t
{
};
struct delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3  : public MulticastDelegate_t
{
};
struct delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0  : public MulticastDelegate_t
{
};
struct delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957  : public MulticastDelegate_t
{
};
struct delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5  : public MulticastDelegate_t
{
};
struct delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710  : public MulticastDelegate_t
{
};
struct delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174  : public MulticastDelegate_t
{
};
struct hook_handle_tADC84A43AFFDADC460E97AB35367B400F2EB2EFC  : public SafeGCHandle_tE8D8C107E75BFC0FAC19C271622895B407785D3E
{
};
struct strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29  : public MulticastDelegate_t
{
};
struct strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E  : public MulticastDelegate_t
{
};
struct strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583  : public MulticastDelegate_t
{
};
struct strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1  : public MulticastDelegate_t
{
};
struct strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C  : public MulticastDelegate_t
{
};
struct strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7  : public MulticastDelegate_t
{
};
struct strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886  : public MulticastDelegate_t
{
};
struct utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 
{
	ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___sp;
};
struct delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933  : public MulticastDelegate_t
{
};
struct delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712  : public MulticastDelegate_t
{
};
struct delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779  : public MulticastDelegate_t
{
};
struct delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172  : public MulticastDelegate_t
{
};
struct delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80  : public MulticastDelegate_t
{
};
struct ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522_StaticFields
{
	bool ___s_isValueWriteAtomic;
};
struct ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517_StaticFields
{
	bool ___s_isValueWriteAtomic;
};
struct ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476_StaticFields
{
	bool ___s_isValueWriteAtomic;
};
struct EqualityComparer_1_tE29394521901AC9A533E45B9990C0246B75F11E0_StaticFields
{
	EqualityComparer_1_tE29394521901AC9A533E45B9990C0246B75F11E0* ___defaultComparer;
};
struct EqualityComparer_1_t622E93DC712395AAC84C30749F11878E251A358F_StaticFields
{
	EqualityComparer_1_t622E93DC712395AAC84C30749F11878E251A358F* ___defaultComparer;
};
struct Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095_StaticFields
{
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___defaultEncoding;
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___unicodeEncoding;
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___bigEndianUnicode;
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___utf7Encoding;
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___utf8Encoding;
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___utf32Encoding;
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___asciiEncoding;
	Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* ___latin1Encoding;
	Dictionary_2_t87EDE08B2E48F793A22DE50D6B3CC2E7EBB2DB54* ___encodings;
	RuntimeObject* ___s_InternalSyncObject;
};
struct String_t_StaticFields
{
	String_t* ___Empty;
};
struct raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_StaticFields
{
	RuntimeObject* ____imp;
	bool ____frozen;
};
struct Boolean_t09A6377A54BE2F9E6985A8149F19234FD7DDFE22_StaticFields
{
	String_t* ___TrueString;
	String_t* ___FalseString;
};
struct Char_t521A6F19B456D956AF452D926C32709DC03D6B17_StaticFields
{
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___s_categoryForLatin1;
};
struct IntPtr_t_StaticFields
{
	intptr_t ___Zero;
};
struct Exception_t_StaticFields
{
	RuntimeObject* ___s_EDILock;
};
struct Type_t_StaticFields
{
	Binder_t91BFCE95A7057FADF4D8A1A342AFE52872246235* ___s_defaultBinder;
	Il2CppChar ___Delimiter;
	TypeU5BU5D_t97234E1129B564EB38B8D85CAC2AD8B5B9522FFB* ___EmptyTypes;
	RuntimeObject* ___Missing;
	MemberFilter_tF644F1AE82F611B677CE1964D5A3277DDA21D553* ___FilterAttribute;
	MemberFilter_tF644F1AE82F611B677CE1964D5A3277DDA21D553* ___FilterName;
	MemberFilter_tF644F1AE82F611B677CE1964D5A3277DDA21D553* ___FilterNameIgnoreCase;
};
#ifdef __clang__
#pragma clang diagnostic pop
#endif
struct DelegateU5BU5D_tC5AB7E8F745616680F337909D3A8E6C722CDF771  : public RuntimeArray
{
	ALIGN_FIELD (8) Delegate_t* m_Items[1];

	inline Delegate_t* GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Delegate_t** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Delegate_t* value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Delegate_t* GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Delegate_t** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Delegate_t* value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
struct StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248  : public RuntimeArray
{
	ALIGN_FIELD (8) String_t* m_Items[1];

	inline String_t* GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline String_t** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, String_t* value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline String_t* GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline String_t** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, String_t* value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
struct ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031  : public RuntimeArray
{
	ALIGN_FIELD (8) uint8_t m_Items[1];

	inline uint8_t GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline uint8_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, uint8_t value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline uint8_t GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline uint8_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, uint8_t value)
	{
		m_Items[index] = value;
	}
};
struct IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832  : public RuntimeArray
{
	ALIGN_FIELD (8) intptr_t m_Items[1];

	inline intptr_t GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline intptr_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, intptr_t value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline intptr_t GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline intptr_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, intptr_t value)
	{
		m_Items[index] = value;
	}
};
struct sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894  : public RuntimeArray
{
	ALIGN_FIELD (8) sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* m_Items[1];

	inline sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};


IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_gshared_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* ReadOnlySpan_1_GetPinnableReference_m365106BC7655B6A45D47673D473A699B5B69DA57_gshared (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_gshared_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_array, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ReadOnlySpan_1_Slice_mEB3D3A427170FC5A0AB734619D4792C299697C89_gshared_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, int32_t ___0_start, int32_t ___1_length, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Span_1__ctor_mE18EBB601FBFA01BA29FE353364700952A9091FE_gshared_inline (Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305* __this, void* ___0_pointer, int32_t ___1_length, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* Span_1_GetPinnableReference_m55DA180AC02A047DAC0626C7B8CBC2E87626DD0C_gshared (Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D Span_1_op_Implicit_mD249188242C0C9D3192A31E9F7FA74C683F05B84_gshared (Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305 ___0_span, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ConcurrentDictionary_2__ctor_m3F802FBA00F30B243C47564955D979C118A3AE42_gshared (ConcurrentDictionary_2_t211FFBACF3ECB80F94746F5D14A5187C12AC7E96* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ConcurrentDictionary_2_set_Item_m7899BE85AED1813DBD60E9E0423FD5C5B0406347_gshared (ConcurrentDictionary_2_t211FFBACF3ECB80F94746F5D14A5187C12AC7E96* __this, intptr_t ___0_key, RuntimeObject* ___1_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* ConcurrentDictionary_2_get_Item_m6549195719E0E1D6562A3D8B62B89AA678051F0B_gshared (ConcurrentDictionary_2_t211FFBACF3ECB80F94746F5D14A5187C12AC7E96* __this, intptr_t ___0_key, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ConcurrentDictionary_2_TryRemove_m648714D9DFA663DE5F768C6908B6BAEE52A0018E_gshared (ConcurrentDictionary_2_t211FFBACF3ECB80F94746F5D14A5187C12AC7E96* __this, intptr_t ___0_key, RuntimeObject** ___1_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27_gshared (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ReadOnlySpan_1_op_Implicit_mCEA7A54A72D5D6EADEFE280B4927119123C8E644_gshared (ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_array, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void ReadOnlySpan_1__ctor_m470D1527EF015478E8677C7BCB52C8410A1DB604_gshared_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, void* ___0_pointer, int32_t ___1_length, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EqualityComparer_1__ctor_mA0D5DF71A4976E2FD4C77C93A93720BEBCBE0DCE_gshared (EqualityComparer_1_t92563A67F1C1ECDC3FE387C46498E2E56B59F3C2* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool Func_4_Invoke_mF686DB15C7046521DFA2350715743471581C6580_gshared_inline (Func_4_t969FE6B8E28BD4E700C6BA2ED5F8794B88E37083* __this, intptr_t ___0_arg1, intptr_t ___1_arg2, int32_t ___2_arg3, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ConcurrentDictionary_2__ctor_m2D937986F9495D8AF5A1FEB1217D83A3AA3FF6D8_gshared (ConcurrentDictionary_2_tF598E45B2A3ECB23FD311D829FB0AB32B1201ACF* __this, RuntimeObject* ___0_comparer, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ConcurrentDictionary_2_TryRemove_m24DC33BB549CD9414E0782A591303A484D2BA255_gshared (ConcurrentDictionary_2_tF598E45B2A3ECB23FD311D829FB0AB32B1201ACF* __this, RuntimeObject* ___0_key, RuntimeObject** ___1_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ConcurrentDictionary_2_set_Item_m95DD60ECF2EBCA55F2EC3B0AC122FE0C0D7D4E39_gshared (ConcurrentDictionary_2_tF598E45B2A3ECB23FD311D829FB0AB32B1201ACF* __this, RuntimeObject* ___0_key, RuntimeObject* ___1_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* ConcurrentDictionary_2_get_Values_m53F5365206C49EF5FD2E74C06F3E7F945CC28946_gshared (ConcurrentDictionary_2_tF598E45B2A3ECB23FD311D829FB0AB32B1201ACF* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void ReadOnlySpan_1__ctor_m0FC0B92549C2968E80B5F75A85F28B96DBFCFD63_gshared_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, uint8_t* ___0_ptr, int32_t ___1_length, const RuntimeMethod* method) ;

IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Attribute__ctor_m79ED1BF1EE36D1E417BA89A0D9F91F8AAD8D19E2 (Attribute_tFDA8EFEFB0711976D22474794576DAF28F7440AA* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Exception__ctor_m9B2BD92CD68916245A75109105D9071C9D430E7F (Exception_t* __this, String_t* ___0_message, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867 (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* sqlite3_New_m5AFB69A71B456CA905E9400C0D8F2A059D8E74B3 (intptr_t ___0_p, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48 (String_t* ___0_s, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_open_m4F057A7A3CDA2344F62847B0F0A551E572092F0E (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___0_filename, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2** ___1_db, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_open_v2_m86ED1D7875270B3598C1B78906C72A6F44CA9A00 (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___0_filename, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2** ___1_db, int32_t ___2_flags, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_vfs, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3__vfs__delete_m01F7630EEDE6F44481D72F21ECEBEED46030B0A1 (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___0_vfs, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_pathname, int32_t ___2_syncdir, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t sqlite3_manual_close_v2_m9913465FCFAA3E200B65FC3E7396D23B00D5351C (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t sqlite3_manual_close_m385D8CE62C53FB890E5878C8FC353A95485514CC (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass263_0__ctor_mA0C4F9F9AEE43DFA9511F900E58F672A0208F64A (U3CU3Ec__DisplayClass263_0_t9EA15DBB375982C7857A45FFC12C73F0B4238EB9* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_log__ctor_m90FE70F302D363779BDD720CE99A7B2A6B07E02A (delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_config_log_m57092048D3F7BE294DC58D14B05B6737DB1CCF94 (delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* ___0_f, RuntimeObject* ___1_v, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_log_m4A7A540937C79BDE846D0A5A574D264F880B88D0 (int32_t ___0_errcode, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_s, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass269_0__ctor_m2C09696A2953271F2CEB026027FB892C3EF47178 (U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_trace__ctor_m363F86F59BCE053BF974338A6E05C69C95C8D189 (delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_trace_m50F2CA43FB6898436D6A2951D781295D390F19C3 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* ___1_f, RuntimeObject* ___2_v, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass271_0__ctor_mD621FC47AF62F2940A6152CDA5BA7F8FC5CE324C (U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_profile__ctor_mACA8FE55EFC53A98950C22BD55D8B8C0A3F6CEDA (delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_profile_m6C48BE4D8091B47292E4B1EC9CE06787430516A7 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* ___1_f, RuntimeObject* ___2_v, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass274_0__ctor_m6B059B3B06DFD76E37692FC3BCC6DAA722C29C3C (U3CU3Ec__DisplayClass274_0_t75B1EC8A8B2CD9CFE76F0CDD3507D686F9C95A31* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_update__ctor_m17032A8DE0B56F6F7EEB38252AA6FF345689EE93 (delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_update_hook_mCBD72FAA0BFA925F97C3230A3FA20A51A2AAD719 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* ___1_f, RuntimeObject* ___2_v, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass275_0__ctor_mE3C57C5B8020F19D1D8240BBF41CC41C413E7838 (U3CU3Ec__DisplayClass275_0_t2E8E6521F4006ACCD7E5F26205A9BC7CBA6A4B9A* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* util_to_utf8_with_z_m3C8DD1FCA689A8E3D7C8AD391BAAFCAC45F6596E (String_t* ___0_sourceText, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_collation__ctor_mF884888642B63BD696BC37E898161379BDAA85AB (delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_create_function_mA7967B3FB878B8FF5420E578A3C312FB021826A0 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_name, int32_t ___2_nArg, int32_t ___3_flags, RuntimeObject* ___4_v, delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* ___5_func, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_create_function_mCFEE0A69952688715C75CFAD8EC16D3FB773589A (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_name, int32_t ___2_nArg, int32_t ___3_flags, RuntimeObject* ___4_v, delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* ___5_func_step, delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* ___6_func_final, const RuntimeMethod* method) ;
inline int32_t ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D*, const RuntimeMethod*))ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_gshared_inline)(__this, method);
}
inline uint8_t* ReadOnlySpan_1_GetPinnableReference_m365106BC7655B6A45D47673D473A699B5B69DA57 (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, const RuntimeMethod* method)
{
	return ((  uint8_t* (*) (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D*, const RuntimeMethod*))ReadOnlySpan_1_GetPinnableReference_m365106BC7655B6A45D47673D473A699B5B69DA57_gshared)(__this, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* Encoding_get_UTF8_m9FA98A53CE96FD6D02982625C5246DD36C1235C9 (const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Encoding_GetString_m42BFF0862341DCD5289A7D75B5D7A22CE9690EAD (Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* __this, uint8_t* ___0_bytes, int32_t ___1_byteCount, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_db_readonly_m628A7A82CB13F774B40F2D3250710453F6FF5462 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_dbName, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_db_filename_m057E3BE2DA4DF24BCFAEE1B7D361FB4D81D00CA9 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_att, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* sqlite3_stmt_From_mA7A4E31058B72E4386C4B74F4B1A93BF446CC207 (intptr_t ___0_p, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___1_db, const RuntimeMethod* method) ;
inline void ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_array, const RuntimeMethod* method)
{
	((  void (*) (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D*, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*, const RuntimeMethod*))ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_gshared_inline)(__this, ___0_array, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_prepare_v2_m88597C248DBAC08DBD376C3432FD9250607D4ACB (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_sql, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___2_stmt, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* ___3_tail, const RuntimeMethod* method) ;
inline ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ReadOnlySpan_1_Slice_mEB3D3A427170FC5A0AB734619D4792C299697C89_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, int32_t ___0_start, int32_t ___1_length, const RuntimeMethod* method)
{
	return ((  ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D (*) (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D*, int32_t, int32_t, const RuntimeMethod*))ReadOnlySpan_1_Slice_mEB3D3A427170FC5A0AB734619D4792C299697C89_gshared_inline)(__this, ___0_start, ___1_length, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* raw_utf8_span_to_string_m1D3AC8DF369A2FFC9AC7B1A7E165F6B4C7234DE2 (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___0_p, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_prepare_v3_m5F2871468C2CC403C7DF6EFFD75BACA36C6FE92F (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_sql, uint32_t ___2_flags, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___3_stmt, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* ___4_tail, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass332_0__ctor_m384555F116520FA2FCFE6E072CB793F3802B1934 (U3CU3Ec__DisplayClass332_0_t1173985050ED9406B43934B3B874C78753CF564A* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_exec__ctor_m64682E5DA3B842E47174CB722B43977DFCF5AE5B (delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool IntPtr_op_Equality_m7D9CDCDE9DC2A0C2C614633F4921E90187FAB271_inline (intptr_t ___0_value1, intptr_t ___1_value2, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* util_from_utf8_z_mA4AD3FF9FAB5CA653DD808B6B17A04FDF7425743 (intptr_t ___0_nativeString, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t sqlite3_stmt_manual_close_m3A3C2F6C6782CB371A60EA7B5372F9CCA721577A (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_complete_m40E74786ADC4CF27376C9AADFCE23F33E939E869 (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___0_sql, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_compileoption_used_m9905722912C4F69D2735DA5349726FB1056BF621 (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___0_s, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_table_column_metadata_mB544F7219F7E00FD72E574C5B1A7011165D4AA93 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_dbName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_tblName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_colName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* ___4_dataType, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* ___5_collSeq, int32_t* ___6_notNull, int32_t* ___7_primaryKey, int32_t* ___8_autoInc, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* sqlite3_stmt_get_db_m31DCFD46B918941ED21CAD5F953201B59EF259CF_inline (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR intptr_t sqlite3_stmt_get_ptr_mF5030B60EB110512D248A632A61AE2F899580B58_inline (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* sqlite3_find_stmt_m6DB56FD791338E8E5524252CE51E90D7D6624FE3 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, intptr_t ___0_p, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject* sqlite3_context_get_user_data_m2B56AA945EBDDC7B032007BA993E2B0D6311E609_inline (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR intptr_t sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_error_m07EEA2401F42F29ACD02EB8950E259A4EB769D21 (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_val, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_text_mB5E6D83F397C4F537EE93458DDB98B15AA8A830B (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_val, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR intptr_t sqlite3_value_get_ptr_mAF588DD3703B49533D61586741960A38A4515AD9_inline (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t String_get_Length_m42625D67623FA5CC7A44D47425CE86FB946542D2_inline (String_t* __this, const RuntimeMethod* method) ;
inline void Span_1__ctor_mE18EBB601FBFA01BA29FE353364700952A9091FE_inline (Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305* __this, void* ___0_pointer, int32_t ___1_length, const RuntimeMethod* method)
{
	((  void (*) (Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305*, void*, int32_t, const RuntimeMethod*))Span_1__ctor_mE18EBB601FBFA01BA29FE353364700952A9091FE_gshared_inline)(__this, ___0_pointer, ___1_length, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t RuntimeHelpers_get_OffsetToStringData_m90A5D27EF88BE9432BF7093B7D7E7A0ACB0A8FBD (const RuntimeMethod* method) ;
inline uint8_t* Span_1_GetPinnableReference_m55DA180AC02A047DAC0626C7B8CBC2E87626DD0C (Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305* __this, const RuntimeMethod* method)
{
	return ((  uint8_t* (*) (Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305*, const RuntimeMethod*))Span_1_GetPinnableReference_m55DA180AC02A047DAC0626C7B8CBC2E87626DD0C_gshared)(__this, method);
}
inline ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D Span_1_op_Implicit_mD249188242C0C9D3192A31E9F7FA74C683F05B84 (Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305 ___0_span, const RuntimeMethod* method)
{
	return ((  ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D (*) (Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305, const RuntimeMethod*))Span_1_op_Implicit_mD249188242C0C9D3192A31E9F7FA74C683F05B84_gshared)(___0_span, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_text_m25D027C3A22B8942CD029A94A14E3B72091F5B9C (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_val, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_text_m0DC3ED8746CA56D5D9531F8C2438240B4A664DD2 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_val, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_parameter_index_m4477224407A1C70D93BB68EF993CAADBC2CD4F28 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_strName, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t sqlite3_backup_manual_close_m16BBF17DEA8FFABBE26C4A1668815FA624897244 (sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* sqlite3_snapshot_From_mFC6D90D41F3A68F30767F7A792A370CFF7D39E83 (intptr_t ___0_p, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_snapshot_manual_close_m49FE953A1E7ACF28AA2C4A2C7EF8DEC9A953D9C8 (sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_blob_open_m16A7C4AD7CCA801A481F634DADC1E866505532C5 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_db_utf8, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_table_utf8, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_col_utf8, int64_t ___4_rowid, int32_t ___5_flags, sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057** ___6_blob, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t sqlite3_blob_manual_close_m32537CD8CDCC89D1CBAD7832CFA559A05C61BA5F (sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass429_0__ctor_m077C7CDEAC013F152AD5A4380DAC6A282842CDF5 (U3CU3Ec__DisplayClass429_0_tF649DDD522F0D50321341692A00F5965CC3248D6* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_authorizer__ctor_m4F3DB7EAF151377921A14384AB7CB7E379B497E2 (delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_set_authorizer_m5DA23A70DA0E64D34F3DBAEBBC59C19B7A76CFFA (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* ___1_f, RuntimeObject* ___2_user_data, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2 (RuntimeObject* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_inline (strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, String_t* ___2_msg, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_inline (strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* __this, RuntimeObject* ___0_user_data, String_t* ___1_s, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_inline (strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* __this, RuntimeObject* ___0_user_data, String_t* ___1_statement, int64_t ___2_ns, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_inline (strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, String_t* ___2_database, String_t* ___3_table, int64_t ___4_rowid, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_inline (strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* __this, RuntimeObject* ___0_user_data, String_t* ___1_s1, String_t* ___2_s2, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_inline (strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* __this, RuntimeObject* ___0_user_data, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___1_values, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___2_names, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_inline (strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, String_t* ___2_param0, String_t* ___3_param1, String_t* ___4_dbName, String_t* ___5_inner_most_trigger_or_view, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SafeHandle__ctor_m23E44C94503043292DCD4E87818082CFC09A7F4B (SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7* __this, intptr_t ___0_invalidHandleValue, bool ___1_ownsHandle, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_backup__ctor_m63CDC567578C17953C38ACA2E9FAAE0BFF778F7C (sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void SafeHandle_SetHandle_m003D64748F9DFBA1E3C0B23798C23BA81AA21C2A_inline (SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7* __this, intptr_t ___0_handle, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_internal_sqlite3_backup_finish_m84D9D7C6128794ADADBF3757E4587590A8749DC1 (intptr_t ___0_p, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_blob__ctor_mF7C7B725FB9FE141587C4BE5A547765610962159 (sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_internal_sqlite3_blob_close_mF4D383511A58F4241E7337A440194216B4505DEC (intptr_t ___0_blob, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_snapshot__ctor_m6CAB94F95FF7CC8925154B4E452ECFD6F16FD6C7 (sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_internal_sqlite3_snapshot_free_mB9410BEFC8368B7ECE442E6CB7D8725A51649FD3 (intptr_t ___0_p, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_stmt__ctor_m8274DE84D8B25F50A07FA11BDB40CA18880774B3 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_add_stmt_mD2483B53A00B53DE52B70C40C475EC191199C73B (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_internal_sqlite3_finalize_m4A8747B9480EA6234BC79A647F029851A3A99A41 (intptr_t ___0_stmt, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_remove_stmt_m4388870B666E82A70AE88284E79AADD62C321BA6 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_s, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_internal_sqlite3_close_v2_m50AD68E0E1ED21F1E91ECF9E2F7D9FD360B18A9D (intptr_t ___0_p, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_dispose_extra_m353F2CCE56ADF182709E81FEEEC40854E41F807B (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_internal_sqlite3_close_m1B069EFBA4CBDA60B0522F50E95833EE8E2F8C10 (intptr_t ___0_p, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3__ctor_m9665E7B91BF88357F8083A085B576DCC73A40B93 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, const RuntimeMethod* method) ;
inline void ConcurrentDictionary_2__ctor_m738B333B82DBBF304A15728C98883F5163204374 (ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476* __this, const RuntimeMethod* method)
{
	((  void (*) (ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476*, const RuntimeMethod*))ConcurrentDictionary_2__ctor_m3F802FBA00F30B243C47564955D979C118A3AE42_gshared)(__this, method);
}
inline void ConcurrentDictionary_2_set_Item_m0B68193309F23091AEE40F02FBB000FAFB452DF8 (ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476* __this, intptr_t ___0_key, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___1_value, const RuntimeMethod* method)
{
	((  void (*) (ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476*, intptr_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, const RuntimeMethod*))ConcurrentDictionary_2_set_Item_m7899BE85AED1813DBD60E9E0423FD5C5B0406347_gshared)(__this, ___0_key, ___1_value, method);
}
inline sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ConcurrentDictionary_2_get_Item_mE58842628CBB5BB71F07874570EC30E7FC721445 (ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476* __this, intptr_t ___0_key, const RuntimeMethod* method)
{
	return ((  sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* (*) (ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476*, intptr_t, const RuntimeMethod*))ConcurrentDictionary_2_get_Item_m6549195719E0E1D6562A3D8B62B89AA678051F0B_gshared)(__this, ___0_key, method);
}
inline bool ConcurrentDictionary_2_TryRemove_m8C588451D72CA78D8618557F8A75D35324158B5D (ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476* __this, intptr_t ___0_key, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___1_value, const RuntimeMethod* method)
{
	return ((  bool (*) (ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476*, intptr_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**, const RuntimeMethod*))ConcurrentDictionary_2_TryRemove_m648714D9DFA663DE5F768C6908B6BAEE52A0018E_gshared)(__this, ___0_key, ___1_value, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 utf8z_FromString_m7BB9CCF1090502FE22763B511942841646A49A2D (String_t* ___0_s, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool IntPtr_op_Inequality_m90EFC9C4CAD9A33E309F2DDF98EE4E1DD253637B_inline (intptr_t ___0_value1, intptr_t ___1_value2, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t Marshal_ReadByte_m40222A943AEA82FBFAC5D4881CABD56DFFBA7085 (intptr_t ___0_ptr, int32_t ___1_ofs, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t util_my_strlen_m19F785A27156B83F63B0351BEEA1D63D47D10E61 (intptr_t ___0_nativeString, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* util_from_utf8_mD7425F48F00DC0C3573A6E3C53744D6C114C52B6 (intptr_t ___0_nativeString, int32_t ___1_size, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void* IntPtr_ToPointer_m1A0612EED3A1C8B8850BE2943CFC42523064B4F6_inline (intptr_t* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* utf8z_GetPinnableReference_m7FC3FFCB77E49E28512035FDEF8CF181E2D39FE5 (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void utf8z__ctor_m4B9DBE66D43FBDB2E4AF2D6DAADBD85A87366FFD_inline (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* __this, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___0_a, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ArgumentException__ctor_m026938A67AF9D36BB7ED27F80425D7194B514465 (ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263* __this, String_t* ___0_message, const RuntimeMethod* method) ;
inline ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27 (const RuntimeMethod* method)
{
	return ((  ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D (*) (const RuntimeMethod*))ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27_gshared)(method);
}
inline ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ReadOnlySpan_1_op_Implicit_mCEA7A54A72D5D6EADEFE280B4927119123C8E644 (ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_array, const RuntimeMethod* method)
{
	return ((  ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D (*) (ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*, const RuntimeMethod*))ReadOnlySpan_1_op_Implicit_mCEA7A54A72D5D6EADEFE280B4927119123C8E644_gshared)(___0_array, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t utf8z_my_strlen_mD6037181019A99D60C76A123A59331DFAA86EE8E (uint8_t* ___0_p, const RuntimeMethod* method) ;
inline void ReadOnlySpan_1__ctor_m470D1527EF015478E8677C7BCB52C8410A1DB604_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, void* ___0_pointer, int32_t ___1_length, const RuntimeMethod* method)
{
	((  void (*) (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D*, void*, int32_t, const RuntimeMethod*))ReadOnlySpan_1__ctor_m470D1527EF015478E8677C7BCB52C8410A1DB604_gshared_inline)(__this, ___0_pointer, ___1_length, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D utf8z_find_zero_terminator_m6B5DD136DDAF627919C425944E08F3DC3BC1F7AB (uint8_t* ___0_p, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 utf8z_FromSpan_mCA9D0C027632A3293CD3FBB43AF9887C124D1E44 (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___0_span, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC GCHandle_Alloc_m3BFD398427352FC756FFE078F01A504B681352EC (RuntimeObject* ___0_value, int32_t ___1_type, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t GCHandle_ToIntPtr_m45294AA913461A070BD555F81103A8BF2E5ED976 (GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC ___0_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC GCHandle_FromIntPtr_mA7848A4285D007CADC52B6272DB243C8FDFD5FAC (intptr_t ___0_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GCHandle_Free_m1320A260E487EB1EA6D95F9E54BFFCB5A4EF83A3 (GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SafeGCHandle__ctor_mB8C029FD49878D0939689FF0B31B855D9279196E (SafeGCHandle_tE8D8C107E75BFC0FAC19C271622895B407785D3E* __this, RuntimeObject* ___0_v, int32_t ___1_typ, const RuntimeMethod* method) ;
inline void EqualityComparer_1__ctor_m98FB31E1349FDCF9F0A3BD1891F322A1AC6CFBC6 (EqualityComparer_1_tE29394521901AC9A533E45B9990C0246B75F11E0* __this, const RuntimeMethod* method)
{
	((  void (*) (EqualityComparer_1_tE29394521901AC9A533E45B9990C0246B75F11E0*, const RuntimeMethod*))EqualityComparer_1__ctor_mA0D5DF71A4976E2FD4C77C93A93720BEBCBE0DCE_gshared)(__this, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t GCHandle_AddrOfPinnedObject_m9C047E154D6F0FE66BE003AB99F0B67A2CA953A6 (GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC* __this, const RuntimeMethod* method) ;
inline bool Func_4_Invoke_mF686DB15C7046521DFA2350715743471581C6580_inline (Func_4_t969FE6B8E28BD4E700C6BA2ED5F8794B88E37083* __this, intptr_t ___0_arg1, intptr_t ___1_arg2, int32_t ___2_arg3, const RuntimeMethod* method)
{
	return ((  bool (*) (Func_4_t969FE6B8E28BD4E700C6BA2ED5F8794B88E37083*, intptr_t, intptr_t, int32_t, const RuntimeMethod*))Func_4_Invoke_mF686DB15C7046521DFA2350715743471581C6580_gshared_inline)(__this, ___0_arg1, ___1_arg2, ___2_arg3, method);
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void FuncName_set_name_m2E558A596C0258BE6EE151C469BCD07A6426A937_inline (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_value, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void FuncName_set_n_mBBC1FE0A2863C200708D2CE3BAB22423DA29259E_inline (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* __this, int32_t ___0_value, const RuntimeMethod* method) ;
inline void EqualityComparer_1__ctor_mF420C788DC290244A73738F4BC6F567E4B8B065D (EqualityComparer_1_t622E93DC712395AAC84C30749F11878E251A358F* __this, const RuntimeMethod* method)
{
	((  void (*) (EqualityComparer_1_t622E93DC712395AAC84C30749F11878E251A358F*, const RuntimeMethod*))EqualityComparer_1__ctor_mA0D5DF71A4976E2FD4C77C93A93720BEBCBE0DCE_gshared)(__this, method);
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t FuncName_get_n_m91D9E17080DD4D1D59BD9879BE61EBCF5C88E0C4_inline (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* FuncName_get_name_mB74437A157D3DF6F5D2A693DC4B2C5DC3E47D648_inline (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CompareBuf__ctor_m6C58AEB15ADFE4A32C2EC9DDDA455E5A929F92E0 (CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA* __this, Func_4_t969FE6B8E28BD4E700C6BA2ED5F8794B88E37083* ___0_f, const RuntimeMethod* method) ;
inline void ConcurrentDictionary_2__ctor_mD8C0B49C9BC53C925E6FAB2B37BADF318BAA208B (ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522* __this, RuntimeObject* ___0_comparer, const RuntimeMethod* method)
{
	((  void (*) (ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522*, RuntimeObject*, const RuntimeMethod*))ConcurrentDictionary_2__ctor_m2D937986F9495D8AF5A1FEB1217D83A3AA3FF6D8_gshared)(__this, ___0_comparer, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CompareFuncName__ctor_m286F43C4A7064F1D2822AFA93BD4950F6158E24B (CompareFuncName_tDD5FD56A7AAA8A632A4B49F594E21250BFB40795* __this, RuntimeObject* ___0_ptrlencmp, const RuntimeMethod* method) ;
inline void ConcurrentDictionary_2__ctor_mC0BBBB7D4FBDEA79D4636FBDB872E0AC54E2C776 (ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517* __this, RuntimeObject* ___0_comparer, const RuntimeMethod* method)
{
	((  void (*) (ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517*, RuntimeObject*, const RuntimeMethod*))ConcurrentDictionary_2__ctor_m2D937986F9495D8AF5A1FEB1217D83A3AA3FF6D8_gshared)(__this, ___0_comparer, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void FuncName__ctor_m7E7689C95B8E84D185844712A71EAE817A2FF240 (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0__name, int32_t ___1__n, const RuntimeMethod* method) ;
inline bool ConcurrentDictionary_2_TryRemove_m1782697D740431D81873A6F2B420EBBDD4FED32D (ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517* __this, FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* ___0_key, RuntimeObject** ___1_value, const RuntimeMethod* method)
{
	return ((  bool (*) (ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517*, FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A*, RuntimeObject**, const RuntimeMethod*))ConcurrentDictionary_2_TryRemove_m24DC33BB549CD9414E0782A591303A484D2BA255_gshared)(__this, ___0_key, ___1_value, method);
}
inline void ConcurrentDictionary_2_set_Item_mB18463C3EF5A1B49665FDDC7A7418F69CA3F69F0 (ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517* __this, FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* ___0_key, RuntimeObject* ___1_value, const RuntimeMethod* method)
{
	((  void (*) (ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517*, FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A*, RuntimeObject*, const RuntimeMethod*))ConcurrentDictionary_2_set_Item_m95DD60ECF2EBCA55F2EC3B0AC122FE0C0D7D4E39_gshared)(__this, ___0_key, ___1_value, method);
}
inline bool ConcurrentDictionary_2_TryRemove_m8220F91E42BAB284E34184616737AAB2A4C9FEF6 (ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_key, RuntimeObject** ___1_value, const RuntimeMethod* method)
{
	return ((  bool (*) (ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522*, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*, RuntimeObject**, const RuntimeMethod*))ConcurrentDictionary_2_TryRemove_m24DC33BB549CD9414E0782A591303A484D2BA255_gshared)(__this, ___0_key, ___1_value, method);
}
inline void ConcurrentDictionary_2_set_Item_m0A8DC29085CC370BCE112BE5175D0766C86B5121 (ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_key, RuntimeObject* ___1_value, const RuntimeMethod* method)
{
	((  void (*) (ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522*, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*, RuntimeObject*, const RuntimeMethod*))ConcurrentDictionary_2_set_Item_m95DD60ECF2EBCA55F2EC3B0AC122FE0C0D7D4E39_gshared)(__this, ___0_key, ___1_value, method);
}
inline RuntimeObject* ConcurrentDictionary_2_get_Values_m2C4385FFCDA16FF0FB436BE7E6127E2F473040FC (ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522* __this, const RuntimeMethod* method)
{
	return ((  RuntimeObject* (*) (ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522*, const RuntimeMethod*))ConcurrentDictionary_2_get_Values_m53F5365206C49EF5FD2E74C06F3E7F945CC28946_gshared)(__this, method);
}
inline RuntimeObject* ConcurrentDictionary_2_get_Values_mF2008A5617C18F3F234996E8F3070D5C9F7ABC10 (ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517* __this, const RuntimeMethod* method)
{
	return ((  RuntimeObject* (*) (ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517*, const RuntimeMethod*))ConcurrentDictionary_2_get_Values_m53F5365206C49EF5FD2E74C06F3E7F945CC28946_gshared)(__this, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC GCHandle_op_Explicit_mA5F28206637454AD677BE13DF86C6152190B6F0F (intptr_t ___0_value, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject* GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_inline (GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_inline (delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_msg, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void commit_hook_info_set__func_m2283E27CCE511047C4028072A7FCC50E1354A281_inline (commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* __this, delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* ___0_value, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void commit_hook_info_set__user_data_mA2C1CEA31C422E0678A0176851D9623090CFE266_inline (commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* __this, RuntimeObject* ___0_value, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* commit_hook_info_get__func_m05BEEED3611D2441FE7BB1D3FA2DC7B53F14B46D_inline (commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject* commit_hook_info_get__user_data_mD65F8F042015B81A6FD9D5CB216FF346F0AB68C6_inline (commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_inline (delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_inline (delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_inline (delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_inline (delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, int64_t ___2_ns, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_inline (delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_inline (delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_database, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_table, int64_t ___4_rowid, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_inline (delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* __this, RuntimeObject* ___0_user_data, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_s1, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_s2, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Type_t* Type_GetTypeFromHandle_m6062B81682F79A4D6DF2640692EE6D9987858C57 (RuntimeTypeHandle_t332A452B8B6179E4469B69525D0FE82A88030F7B ___0_handle, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Marshal_SizeOf_mED64846722033D6F60C2973CA604B7C2D7D4A1B7 (Type_t* ___0_t, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t Marshal_ReadIntPtr_m576E200A849BE7A6BC688058AA869B12B30D970F (intptr_t ___0_ptr, int32_t ___1_ofs, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_inline (delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* __this, RuntimeObject* ___0_user_data, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___1_values, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___2_names, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t Marshal_ReadIntPtr_m6E8694E5CB4FE576B3CAE1A002B03C211D393826 (intptr_t ___0_ptr, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void agg_sqlite3_context__ctor_m2FD4EE33CC38CC17ADF6466615FE093912A8C168 (agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07* __this, RuntimeObject* ___0_v, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC GCHandle_Alloc_m845AB5ED62859B099C023F34C05BEAEDB4AFE27D (RuntimeObject* ___0_value, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR intptr_t GCHandle_op_Explicit_m03DD8D9FB45D565431455A6EE5C30A87305EF73C_inline (GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC ___0_value, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Marshal_WriteIntPtr_m3AA18248A64282B1CFB4FF0B13678B2E08DADA36 (intptr_t ___0_ptr, intptr_t ___1_val, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void agg_sqlite3_context_fix_ptr_m1A20B24DEF5D9E1199E5A60A31376373947B024E (agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07* __this, intptr_t ___0_p, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void scalar_sqlite3_context__ctor_mE4C296D9999B1489512214B36C283EFBBA65B72F (scalar_sqlite3_context_t520AE3E6198561DEF6B32657E820E9B8FBDE0509* __this, intptr_t ___0_p, RuntimeObject* ___1_v, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_value__ctor_m653B23B5B2873FC7E8AB166AAE5E00151A929676 (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* __this, intptr_t ___0_p, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_inline (delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* function_hook_info_get_context_mAB02E6F7CA2611530E05A39863DBD6FE1CF323B5 (function_hook_info_t2884BE4D2AF455A249E6382005E52E62CDB7E017* __this, intptr_t ___0_context, intptr_t ___1_agg_context, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_inline (delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_inline (delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_context__ctor_mD3A1FF371768B7E9C88EC86EFE32952B8D89D71F (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void sqlite3_context_set_context_ptr_m75037F45D04497B679E79EF821D191289E5917A3_inline (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* __this, intptr_t ___0_p, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_inline (delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_param0, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_param1, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___4_dbName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___5_inner_most_trigger_or_view, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void EntryPointAttribute_set_Name_m865E8DAD340EDE93960B660C46F3CA0602439808_inline (EntryPointAttribute_t153F60F730CBAB2CE127F07F09554695F9B9D075* __this, String_t* ___0_value, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool GCHandle_get_IsAllocated_m241908103D8D867E11CCAB73C918729825E86843_inline (GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162 (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* __this, String_t* ___0_message, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool GCHandle_CanDereferenceHandle_mAAAC42D1268CEF3FDD040A3D1574773D08140579_inline (intptr_t ___0_handle, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject* GCHandle_GetRef_mAC7E58E62417209DC41C99F66BA70F0C3AA18DA8_inline (intptr_t ___0_handle, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* GCHandle_GetTarget_mE0AF851834410E2AEA6285B2497751570236C794 (intptr_t ___0_handle, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR uint8_t* Array_GetRawSzArrayData_m2F8F5B2A381AEF971F12866D9C0A6C4FBA59F6BB_inline (RuntimeArray* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ThrowHelper_ThrowArgumentOutOfRangeException_mD7D90276EDCDF9394A8EA635923E3B48BB71BD56 (const RuntimeMethod* method) ;
inline void ReadOnlySpan_1__ctor_m0FC0B92549C2968E80B5F75A85F28B96DBFCFD63_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, uint8_t* ___0_ptr, int32_t ___1_length, const RuntimeMethod* method)
{
	((  void (*) (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D*, uint8_t*, int32_t, const RuntimeMethod*))ReadOnlySpan_1__ctor_m0FC0B92549C2968E80B5F75A85F28B96DBFCFD63_gshared_inline)(__this, ___0_ptr, ___1_length, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void* IntPtr_op_Explicit_m2728CBA081E79B97DDCF1D4FAD77B309CA1E94BF (intptr_t ___0_value, const RuntimeMethod* method) ;
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117211
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EmbeddedAttribute__ctor_m04352FA10E125A83975C61DBB73A183DE5C24402 (EmbeddedAttribute_tA4674B43E8DDF11FB95382E2DF287778F940BFBA* __this, const RuntimeMethod* method) 
{
	{
		Attribute__ctor_m79ED1BF1EE36D1E417BA89A0D9F91F8AAD8D19E2(__this, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117212
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IsReadOnlyAttribute__ctor_m49B040BAE122324446D79070C07C7DC866FB36A9 (IsReadOnlyAttribute_tB8CFB53741972E7671A35C8614A6613F318DBB0A* __this, const RuntimeMethod* method) 
{
	{
		Attribute__ctor_m79ED1BF1EE36D1E417BA89A0D9F91F8AAD8D19E2(__this, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117213
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IsByRefLikeAttribute__ctor_mB685E19152B9E8C4703D7C87C80289435E9ADC98 (IsByRefLikeAttribute_tE3DF4234BFCF4858A0D7C555BB5DC3A332D72370* __this, const RuntimeMethod* method) 
{
	{
		Attribute__ctor_m79ED1BF1EE36D1E417BA89A0D9F91F8AAD8D19E2(__this, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
int32_t strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_Multicast(strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* __this, RuntimeObject* ___0_user_data, String_t* ___1_s1, String_t* ___2_s2, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	int32_t retVal = 0;
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* currentDelegate = reinterpret_cast<strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E*>(delegatesToInvoke[i]);
		typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, String_t*, String_t*, const RuntimeMethod*);
		retVal = ((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, ___1_s1, ___2_s2, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
	return retVal;
}
int32_t strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_OpenInst(strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* __this, RuntimeObject* ___0_user_data, String_t* ___1_s1, String_t* ___2_s2, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, String_t*, String_t*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_s1, ___2_s2, method);
}
int32_t strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_OpenStatic(strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* __this, RuntimeObject* ___0_user_data, String_t* ___1_s1, String_t* ___2_s2, const RuntimeMethod* method)
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, String_t*, String_t*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_s1, ___2_s2, method);
}
int32_t strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_OpenVirtual(strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* __this, RuntimeObject* ___0_user_data, String_t* ___1_s1, String_t* ___2_s2, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return VirtualFuncInvoker2< int32_t, String_t*, String_t* >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data, ___1_s1, ___2_s2);
}
int32_t strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_OpenInterface(strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* __this, RuntimeObject* ___0_user_data, String_t* ___1_s1, String_t* ___2_s2, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return InterfaceFuncInvoker2< int32_t, String_t*, String_t* >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data, ___1_s1, ___2_s2);
}
int32_t strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_OpenGenericVirtual(strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* __this, RuntimeObject* ___0_user_data, String_t* ___1_s1, String_t* ___2_s2, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericVirtualFuncInvoker2< int32_t, String_t*, String_t* >::Invoke(method, ___0_user_data, ___1_s1, ___2_s2);
}
int32_t strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_OpenGenericInterface(strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* __this, RuntimeObject* ___0_user_data, String_t* ___1_s1, String_t* ___2_s2, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericInterfaceFuncInvoker2< int32_t, String_t*, String_t* >::Invoke(method, ___0_user_data, ___1_s1, ___2_s2);
}
// Method Definition Index: 117214
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_collation__ctor_mF581AF9E79F16A5F984001D7024B78323A305D77 (strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 3;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 2;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_Multicast;
}
// Method Definition Index: 117215
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537 (strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* __this, RuntimeObject* ___0_user_data, String_t* ___1_s1, String_t* ___2_s2, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, String_t*, String_t*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_s1, ___2_s2, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117216
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* strdelegate_collation_BeginInvoke_m69631D4A6411BC93DD99EA58E463AE3454037C6E (strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* __this, RuntimeObject* ___0_user_data, String_t* ___1_s1, String_t* ___2_s2, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___3_callback, RuntimeObject* ___4_object, const RuntimeMethod* method) 
{
	void *__d_args[4] = {0};
	__d_args[0] = ___0_user_data;
	__d_args[1] = ___1_s1;
	__d_args[2] = ___2_s2;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___3_callback, (RuntimeObject*)___4_object);
}
// Method Definition Index: 117217
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t strdelegate_collation_EndInvoke_mA712FEDF069F219CA199B7E07BF6A220764CFF60 (strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
	return *(int32_t*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
void strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_Multicast(strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, String_t* ___2_database, String_t* ___3_table, int64_t ___4_rowid, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* currentDelegate = reinterpret_cast<strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886*>(delegatesToInvoke[i]);
		typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, String_t*, String_t*, int64_t, const RuntimeMethod*);
		((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
}
void strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_OpenInst(strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, String_t* ___2_database, String_t* ___3_table, int64_t ___4_rowid, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef void (*FunctionPointerType) (RuntimeObject*, int32_t, String_t*, String_t*, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid, method);
}
void strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_OpenStatic(strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, String_t* ___2_database, String_t* ___3_table, int64_t ___4_rowid, const RuntimeMethod* method)
{
	typedef void (*FunctionPointerType) (RuntimeObject*, int32_t, String_t*, String_t*, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid, method);
}
void strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_OpenVirtual(strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, String_t* ___2_database, String_t* ___3_table, int64_t ___4_rowid, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	VirtualActionInvoker4< int32_t, String_t*, String_t*, int64_t >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid);
}
void strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_OpenInterface(strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, String_t* ___2_database, String_t* ___3_table, int64_t ___4_rowid, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	InterfaceActionInvoker4< int32_t, String_t*, String_t*, int64_t >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid);
}
void strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_OpenGenericVirtual(strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, String_t* ___2_database, String_t* ___3_table, int64_t ___4_rowid, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericVirtualActionInvoker4< int32_t, String_t*, String_t*, int64_t >::Invoke(method, ___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid);
}
void strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_OpenGenericInterface(strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, String_t* ___2_database, String_t* ___3_table, int64_t ___4_rowid, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericInterfaceActionInvoker4< int32_t, String_t*, String_t*, int64_t >::Invoke(method, ___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid);
}
// Method Definition Index: 117218
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_update__ctor_m6DAB4AF442BA9C4A359D5892DC9F9022D0D6B164 (strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 5;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 4;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_Multicast;
}
// Method Definition Index: 117219
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1 (strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, String_t* ___2_database, String_t* ___3_table, int64_t ___4_rowid, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, String_t*, String_t*, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117220
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* strdelegate_update_BeginInvoke_m8554D2A1EEA5064ABC7BC7FCAD5271FBA8EC2714 (strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, String_t* ___2_database, String_t* ___3_table, int64_t ___4_rowid, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___5_callback, RuntimeObject* ___6_object, const RuntimeMethod* method) 
{
	void *__d_args[6] = {0};
	__d_args[0] = ___0_user_data;
	__d_args[1] = Box(il2cpp_defaults.int32_class, &___1_type);
	__d_args[2] = ___2_database;
	__d_args[3] = ___3_table;
	__d_args[4] = Box(il2cpp_defaults.int64_class, &___4_rowid);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___5_callback, (RuntimeObject*)___6_object);
}
// Method Definition Index: 117221
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_update_EndInvoke_m7B63A257022A1CF7A035C9F0490FC5D55886DAD0 (strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
void strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_Multicast(strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, String_t* ___2_msg, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* currentDelegate = reinterpret_cast<strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1*>(delegatesToInvoke[i]);
		typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, String_t*, const RuntimeMethod*);
		((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, ___1_errorCode, ___2_msg, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
}
void strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_OpenInst(strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, String_t* ___2_msg, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef void (*FunctionPointerType) (RuntimeObject*, int32_t, String_t*, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_errorCode, ___2_msg, method);
}
void strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_OpenStatic(strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, String_t* ___2_msg, const RuntimeMethod* method)
{
	typedef void (*FunctionPointerType) (RuntimeObject*, int32_t, String_t*, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_errorCode, ___2_msg, method);
}
void strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_OpenVirtual(strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, String_t* ___2_msg, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	VirtualActionInvoker2< int32_t, String_t* >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data, ___1_errorCode, ___2_msg);
}
void strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_OpenInterface(strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, String_t* ___2_msg, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	InterfaceActionInvoker2< int32_t, String_t* >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data, ___1_errorCode, ___2_msg);
}
void strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_OpenGenericVirtual(strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, String_t* ___2_msg, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericVirtualActionInvoker2< int32_t, String_t* >::Invoke(method, ___0_user_data, ___1_errorCode, ___2_msg);
}
void strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_OpenGenericInterface(strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, String_t* ___2_msg, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericInterfaceActionInvoker2< int32_t, String_t* >::Invoke(method, ___0_user_data, ___1_errorCode, ___2_msg);
}
// Method Definition Index: 117222
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_log__ctor_mA6A776E3FA150594008BA77EEBAA045F18E13A61 (strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 3;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 2;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_Multicast;
}
// Method Definition Index: 117223
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B (strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, String_t* ___2_msg, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, String_t*, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_errorCode, ___2_msg, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117224
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* strdelegate_log_BeginInvoke_mAE707A55CAD1915A09BAF20361BE7470BEA98AD8 (strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, String_t* ___2_msg, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___3_callback, RuntimeObject* ___4_object, const RuntimeMethod* method) 
{
	void *__d_args[4] = {0};
	__d_args[0] = ___0_user_data;
	__d_args[1] = Box(il2cpp_defaults.int32_class, &___1_errorCode);
	__d_args[2] = ___2_msg;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___3_callback, (RuntimeObject*)___4_object);
}
// Method Definition Index: 117225
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_log_EndInvoke_mCABD24A69C4952CDE4C1103182FFA6553A5F5A55 (strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
int32_t strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_Multicast(strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, String_t* ___2_param0, String_t* ___3_param1, String_t* ___4_dbName, String_t* ___5_inner_most_trigger_or_view, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	int32_t retVal = 0;
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* currentDelegate = reinterpret_cast<strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29*>(delegatesToInvoke[i]);
		typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, String_t*, String_t*, String_t*, String_t*, const RuntimeMethod*);
		retVal = ((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
	return retVal;
}
int32_t strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_OpenInst(strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, String_t* ___2_param0, String_t* ___3_param1, String_t* ___4_dbName, String_t* ___5_inner_most_trigger_or_view, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, int32_t, String_t*, String_t*, String_t*, String_t*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view, method);
}
int32_t strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_OpenStatic(strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, String_t* ___2_param0, String_t* ___3_param1, String_t* ___4_dbName, String_t* ___5_inner_most_trigger_or_view, const RuntimeMethod* method)
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, int32_t, String_t*, String_t*, String_t*, String_t*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view, method);
}
int32_t strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_OpenVirtual(strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, String_t* ___2_param0, String_t* ___3_param1, String_t* ___4_dbName, String_t* ___5_inner_most_trigger_or_view, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return VirtualFuncInvoker5< int32_t, int32_t, String_t*, String_t*, String_t*, String_t* >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view);
}
int32_t strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_OpenInterface(strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, String_t* ___2_param0, String_t* ___3_param1, String_t* ___4_dbName, String_t* ___5_inner_most_trigger_or_view, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return InterfaceFuncInvoker5< int32_t, int32_t, String_t*, String_t*, String_t*, String_t* >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view);
}
int32_t strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_OpenGenericVirtual(strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, String_t* ___2_param0, String_t* ___3_param1, String_t* ___4_dbName, String_t* ___5_inner_most_trigger_or_view, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericVirtualFuncInvoker5< int32_t, int32_t, String_t*, String_t*, String_t*, String_t* >::Invoke(method, ___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view);
}
int32_t strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_OpenGenericInterface(strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, String_t* ___2_param0, String_t* ___3_param1, String_t* ___4_dbName, String_t* ___5_inner_most_trigger_or_view, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericInterfaceFuncInvoker5< int32_t, int32_t, String_t*, String_t*, String_t*, String_t* >::Invoke(method, ___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view);
}
// Method Definition Index: 117226
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_authorizer__ctor_mC310945C34FFB1A7E592B316C21170BF9D6EFEBF (strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 6;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 5;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_Multicast;
}
// Method Definition Index: 117227
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34 (strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, String_t* ___2_param0, String_t* ___3_param1, String_t* ___4_dbName, String_t* ___5_inner_most_trigger_or_view, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, String_t*, String_t*, String_t*, String_t*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117228
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* strdelegate_authorizer_BeginInvoke_mA29CB4EBE0BE427BBD07A270A286A59F841D7B39 (strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, String_t* ___2_param0, String_t* ___3_param1, String_t* ___4_dbName, String_t* ___5_inner_most_trigger_or_view, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___6_callback, RuntimeObject* ___7_object, const RuntimeMethod* method) 
{
	void *__d_args[7] = {0};
	__d_args[0] = ___0_user_data;
	__d_args[1] = Box(il2cpp_defaults.int32_class, &___1_action_code);
	__d_args[2] = ___2_param0;
	__d_args[3] = ___3_param1;
	__d_args[4] = ___4_dbName;
	__d_args[5] = ___5_inner_most_trigger_or_view;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___6_callback, (RuntimeObject*)___7_object);
}
// Method Definition Index: 117229
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t strdelegate_authorizer_EndInvoke_m36EA782A4DAE1730EEE235C66BB2193573403212 (strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
	return *(int32_t*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
void strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_Multicast(strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* __this, RuntimeObject* ___0_user_data, String_t* ___1_s, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* currentDelegate = reinterpret_cast<strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7*>(delegatesToInvoke[i]);
		typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, String_t*, const RuntimeMethod*);
		((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, ___1_s, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
}
void strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_OpenInst(strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* __this, RuntimeObject* ___0_user_data, String_t* ___1_s, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef void (*FunctionPointerType) (RuntimeObject*, String_t*, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_s, method);
}
void strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_OpenStatic(strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* __this, RuntimeObject* ___0_user_data, String_t* ___1_s, const RuntimeMethod* method)
{
	typedef void (*FunctionPointerType) (RuntimeObject*, String_t*, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_s, method);
}
void strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_OpenVirtual(strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* __this, RuntimeObject* ___0_user_data, String_t* ___1_s, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	VirtualActionInvoker1< String_t* >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data, ___1_s);
}
void strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_OpenInterface(strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* __this, RuntimeObject* ___0_user_data, String_t* ___1_s, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	InterfaceActionInvoker1< String_t* >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data, ___1_s);
}
void strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_OpenGenericVirtual(strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* __this, RuntimeObject* ___0_user_data, String_t* ___1_s, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericVirtualActionInvoker1< String_t* >::Invoke(method, ___0_user_data, ___1_s);
}
void strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_OpenGenericInterface(strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* __this, RuntimeObject* ___0_user_data, String_t* ___1_s, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericInterfaceActionInvoker1< String_t* >::Invoke(method, ___0_user_data, ___1_s);
}
// Method Definition Index: 117230
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_trace__ctor_m8E9CA541DAA839E4E00AEBF75B6269D6E9D98450 (strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 2;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 1;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_Multicast;
}
// Method Definition Index: 117231
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779 (strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* __this, RuntimeObject* ___0_user_data, String_t* ___1_s, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, String_t*, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_s, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117232
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* strdelegate_trace_BeginInvoke_mD8F9ED0FFD62D78C22046CA460049BF52606A766 (strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* __this, RuntimeObject* ___0_user_data, String_t* ___1_s, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___2_callback, RuntimeObject* ___3_object, const RuntimeMethod* method) 
{
	void *__d_args[3] = {0};
	__d_args[0] = ___0_user_data;
	__d_args[1] = ___1_s;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___2_callback, (RuntimeObject*)___3_object);
}
// Method Definition Index: 117233
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_trace_EndInvoke_mA45FAB698B9AB59523F3CD269DA6736E096E986F (strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
void strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_Multicast(strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* __this, RuntimeObject* ___0_user_data, String_t* ___1_statement, int64_t ___2_ns, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* currentDelegate = reinterpret_cast<strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C*>(delegatesToInvoke[i]);
		typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, String_t*, int64_t, const RuntimeMethod*);
		((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, ___1_statement, ___2_ns, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
}
void strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_OpenInst(strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* __this, RuntimeObject* ___0_user_data, String_t* ___1_statement, int64_t ___2_ns, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef void (*FunctionPointerType) (RuntimeObject*, String_t*, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_statement, ___2_ns, method);
}
void strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_OpenStatic(strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* __this, RuntimeObject* ___0_user_data, String_t* ___1_statement, int64_t ___2_ns, const RuntimeMethod* method)
{
	typedef void (*FunctionPointerType) (RuntimeObject*, String_t*, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_statement, ___2_ns, method);
}
void strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_OpenVirtual(strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* __this, RuntimeObject* ___0_user_data, String_t* ___1_statement, int64_t ___2_ns, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	VirtualActionInvoker2< String_t*, int64_t >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data, ___1_statement, ___2_ns);
}
void strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_OpenInterface(strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* __this, RuntimeObject* ___0_user_data, String_t* ___1_statement, int64_t ___2_ns, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	InterfaceActionInvoker2< String_t*, int64_t >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data, ___1_statement, ___2_ns);
}
void strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_OpenGenericVirtual(strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* __this, RuntimeObject* ___0_user_data, String_t* ___1_statement, int64_t ___2_ns, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericVirtualActionInvoker2< String_t*, int64_t >::Invoke(method, ___0_user_data, ___1_statement, ___2_ns);
}
void strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_OpenGenericInterface(strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* __this, RuntimeObject* ___0_user_data, String_t* ___1_statement, int64_t ___2_ns, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericInterfaceActionInvoker2< String_t*, int64_t >::Invoke(method, ___0_user_data, ___1_statement, ___2_ns);
}
// Method Definition Index: 117234
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_profile__ctor_m616250D15C9B8988A290BADA57FCA9BF4FA80466 (strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 3;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 2;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_Multicast;
}
// Method Definition Index: 117235
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4 (strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* __this, RuntimeObject* ___0_user_data, String_t* ___1_statement, int64_t ___2_ns, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, String_t*, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_statement, ___2_ns, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117236
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* strdelegate_profile_BeginInvoke_m5D185B48B413256C262402E93FD6ACF34C4ECF8A (strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* __this, RuntimeObject* ___0_user_data, String_t* ___1_statement, int64_t ___2_ns, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___3_callback, RuntimeObject* ___4_object, const RuntimeMethod* method) 
{
	void *__d_args[4] = {0};
	__d_args[0] = ___0_user_data;
	__d_args[1] = ___1_statement;
	__d_args[2] = Box(il2cpp_defaults.int64_class, &___2_ns);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___3_callback, (RuntimeObject*)___4_object);
}
// Method Definition Index: 117237
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_profile_EndInvoke_m44B4E88A500ED49CC0B7931CE0953DE31B55C453 (strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
int32_t strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_Multicast(strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* __this, RuntimeObject* ___0_user_data, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___1_values, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___2_names, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	int32_t retVal = 0;
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* currentDelegate = reinterpret_cast<strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583*>(delegatesToInvoke[i]);
		typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*, const RuntimeMethod*);
		retVal = ((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, ___1_values, ___2_names, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
	return retVal;
}
int32_t strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_OpenInst(strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* __this, RuntimeObject* ___0_user_data, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___1_values, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___2_names, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_values, ___2_names, method);
}
int32_t strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_OpenStatic(strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* __this, RuntimeObject* ___0_user_data, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___1_values, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___2_names, const RuntimeMethod* method)
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_values, ___2_names, method);
}
int32_t strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_OpenVirtual(strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* __this, RuntimeObject* ___0_user_data, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___1_values, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___2_names, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return VirtualFuncInvoker2< int32_t, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data, ___1_values, ___2_names);
}
int32_t strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_OpenInterface(strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* __this, RuntimeObject* ___0_user_data, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___1_values, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___2_names, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return InterfaceFuncInvoker2< int32_t, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data, ___1_values, ___2_names);
}
int32_t strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_OpenGenericVirtual(strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* __this, RuntimeObject* ___0_user_data, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___1_values, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___2_names, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericVirtualFuncInvoker2< int32_t, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* >::Invoke(method, ___0_user_data, ___1_values, ___2_names);
}
int32_t strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_OpenGenericInterface(strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* __this, RuntimeObject* ___0_user_data, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___1_values, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___2_names, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericInterfaceFuncInvoker2< int32_t, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* >::Invoke(method, ___0_user_data, ___1_values, ___2_names);
}
// Method Definition Index: 117238
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void strdelegate_exec__ctor_m57C8B8AD8E009D95BA068CFD59AF5B538E6F8546 (strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 3;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 2;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_Multicast;
}
// Method Definition Index: 117239
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6 (strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* __this, RuntimeObject* ___0_user_data, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___1_values, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___2_names, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_values, ___2_names, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117240
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* strdelegate_exec_BeginInvoke_mD092C6FE4DF919774854345FE5AD2908CA01FB59 (strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* __this, RuntimeObject* ___0_user_data, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___1_values, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___2_names, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___3_callback, RuntimeObject* ___4_object, const RuntimeMethod* method) 
{
	void *__d_args[4] = {0};
	__d_args[0] = ___0_user_data;
	__d_args[1] = ___1_values;
	__d_args[2] = ___2_names;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___3_callback, (RuntimeObject*)___4_object);
}
// Method Definition Index: 117241
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t strdelegate_exec_EndInvoke_mAB7A56B39F061F647DA3AC0FB5B7DFD7F4CE9B1D (strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
	return *(int32_t*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117242
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw__cctor_m5D18881FAB8F319A1C39BE653C44D428334CD77A (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		((raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_StaticFields*)il2cpp_codegen_static_fields_for(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var))->____frozen = (bool)0;
		return;
	}
}
// Method Definition Index: 117243
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_SetProvider_m7C26EBDAA36E936C697456B816A89402B01D6BA1 (RuntimeObject* ___0_imp, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		bool L_0 = ((raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_StaticFields*)il2cpp_codegen_static_fields_for(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var))->____frozen;
		if (!L_0)
		{
			goto IL_0008;
		}
	}
	{
		return;
	}

IL_0008:
	{
		RuntimeObject* L_1 = ___0_imp;
		NullCheck(L_1);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker0< int32_t >::Invoke(10, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_1);
		RuntimeObject* L_3 = ___0_imp;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		((raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_StaticFields*)il2cpp_codegen_static_fields_for(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var))->____imp = L_3;
		Il2CppCodeGenWriteBarrier((void**)(&((raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_StaticFields*)il2cpp_codegen_static_fields_for(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var))->____imp), (void*)L_3);
		return;
	}
}
// Method Definition Index: 117244
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_FreezeProvider_mAC8103810FCF5B36BE5B7801CBFF44C2E7902301 (bool ___0_b, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		bool L_0 = ___0_b;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		((raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_StaticFields*)il2cpp_codegen_static_fields_for(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var))->____frozen = L_0;
		return;
	}
}
// Method Definition Index: 117245
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867 (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0 = ((raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_StaticFields*)il2cpp_codegen_static_fields_for(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var))->____imp;
		if (L_0)
		{
			goto IL_0012;
		}
	}
	{
		Exception_t* L_1 = (Exception_t*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Exception_t_il2cpp_TypeInfo_var)));
		Exception__ctor_m9B2BD92CD68916245A75109105D9071C9D430E7F(L_1, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralF165B374B6AD91BF7008BDDE41B21D6BC0612DB5)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_1, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867_RuntimeMethod_var)));
	}

IL_0012:
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_2 = ((raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_StaticFields*)il2cpp_codegen_static_fields_for(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var))->____imp;
		return L_2;
	}
}
// Method Definition Index: 117246
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* raw_GetNativeLibraryName_m805A415BDEE76604285919EF4A30B8D5BD827569 (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		NullCheck(L_0);
		String_t* L_1;
		L_1 = InterfaceFuncInvoker0< String_t* >::Invoke(0, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0);
		return L_1;
	}
}
// Method Definition Index: 117247
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_open_m4F057A7A3CDA2344F62847B0F0A551E572092F0E (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___0_filename, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2** ___1_db, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_1 = ___0_filename;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker2< int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, intptr_t* >::Invoke(1, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, (&V_0));
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2** L_3 = ___1_db;
		intptr_t L_4 = V_0;
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_5;
		L_5 = sqlite3_New_m5AFB69A71B456CA905E9400C0D8F2A059D8E74B3(L_4, NULL);
		*((sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2**)L_3) = (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*)L_5;
		Il2CppCodeGenWriteBarrier((void**)(sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2**)L_3, (void*)(sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*)L_5);
		return L_2;
	}
}
// Method Definition Index: 117248
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_open_mA163A4F114E920A0DFCCD4E4AB3015647C493DD4 (String_t* ___0_filename, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2** ___1_db, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		String_t* L_0 = ___0_filename;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_1;
		L_1 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_0, NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2** L_2 = ___1_db;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_3;
		L_3 = raw_sqlite3_open_m4F057A7A3CDA2344F62847B0F0A551E572092F0E(L_1, L_2, NULL);
		return L_3;
	}
}
// Method Definition Index: 117249
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_open_v2_m86ED1D7875270B3598C1B78906C72A6F44CA9A00 (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___0_filename, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2** ___1_db, int32_t ___2_flags, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_vfs, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_1 = ___0_filename;
		int32_t L_2 = ___2_flags;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3 = ___3_vfs;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker4< int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, intptr_t*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(2, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, (&V_0), L_2, L_3);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2** L_5 = ___1_db;
		intptr_t L_6 = V_0;
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_7;
		L_7 = sqlite3_New_m5AFB69A71B456CA905E9400C0D8F2A059D8E74B3(L_6, NULL);
		*((sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2**)L_5) = (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*)L_7;
		Il2CppCodeGenWriteBarrier((void**)(sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2**)L_5, (void*)(sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*)L_7);
		return L_4;
	}
}
// Method Definition Index: 117250
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_open_v2_m84C9D06FA7F08726BDBD1610AB78EE77D762C98B (String_t* ___0_filename, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2** ___1_db, int32_t ___2_flags, String_t* ___3_vfs, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		String_t* L_0 = ___0_filename;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_1;
		L_1 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_0, NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2** L_2 = ___1_db;
		int32_t L_3 = ___2_flags;
		String_t* L_4 = ___3_vfs;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_5;
		L_5 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_4, NULL);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_6;
		L_6 = raw_sqlite3_open_v2_m86ED1D7875270B3598C1B78906C72A6F44CA9A00(L_1, L_2, L_3, L_5, NULL);
		return L_6;
	}
}
// Method Definition Index: 117251
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3__vfs__delete_m01F7630EEDE6F44481D72F21ECEBEED46030B0A1 (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___0_vfs, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_pathname, int32_t ___2_syncdir, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_1 = ___0_vfs;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___1_pathname;
		int32_t L_3 = ___2_syncdir;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int32_t >::Invoke(7, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117252
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3__vfs__delete_m23AC64C55998E29F2F37BC385FE0C4EBCD78E684 (String_t* ___0_vfs, String_t* ___1_pathname, int32_t ___2_syncdir, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		String_t* L_0 = ___0_vfs;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_1;
		L_1 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_0, NULL);
		String_t* L_2 = ___1_pathname;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_2, NULL);
		int32_t L_4 = ___2_syncdir;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_5;
		L_5 = raw_sqlite3__vfs__delete_m01F7630EEDE6F44481D72F21ECEBEED46030B0A1(L_1, L_3, L_4, NULL);
		return L_5;
	}
}
// Method Definition Index: 117253
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_internal_sqlite3_close_v2_m50AD68E0E1ED21F1E91ECF9E2F7D9FD360B18A9D (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		intptr_t L_1 = ___0_p;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, intptr_t >::Invoke(3, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117254
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_internal_sqlite3_close_m1B069EFBA4CBDA60B0522F50E95833EE8E2F8C10 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		intptr_t L_1 = ___0_p;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, intptr_t >::Invoke(4, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117255
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_close_v2_mA6255CF7DC6E915181B599AFCC91D56DEA531599 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, const RuntimeMethod* method) 
{
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_0 = ___0_db;
		NullCheck(L_0);
		int32_t L_1;
		L_1 = sqlite3_manual_close_v2_m9913465FCFAA3E200B65FC3E7396D23B00D5351C(L_0, NULL);
		return L_1;
	}
}
// Method Definition Index: 117256
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_close_m22013C25CAC7C612328713456320806B4987C060 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, const RuntimeMethod* method) 
{
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_0 = ___0_db;
		NullCheck(L_0);
		int32_t L_1;
		L_1 = sqlite3_manual_close_m385D8CE62C53FB890E5878C8FC353A95485514CC(L_0, NULL);
		return L_1;
	}
}
// Method Definition Index: 117257
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_enable_shared_cache_m9137F4FD3C4CDBCB49317982F6018F9F6C414988 (int32_t ___0_enable, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		int32_t L_1 = ___0_enable;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, int32_t >::Invoke(5, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117258
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_interrupt_mC2937A7191101A3329770DA19BD4C81102DD9624 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		NullCheck(L_0);
		InterfaceActionInvoker1< sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* >::Invoke(6, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return;
	}
}
// Method Definition Index: 117259
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_config_log_m57092048D3F7BE294DC58D14B05B6737DB1CCF94 (delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* ___0_f, RuntimeObject* ___1_v, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* L_1 = ___0_f;
		RuntimeObject* L_2 = ___1_v;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712*, RuntimeObject* >::Invoke(83, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117260
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_config_log_m655A72486ADBE8417FFA2A98F9D2724DB598C939 (strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* ___0_f, RuntimeObject* ___1_v, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass263_0_U3Csqlite3_config_logU3Eb__0_m8B4286DB43D892CCAC433BFC538CE4D0117CC508_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass263_0_t9EA15DBB375982C7857A45FFC12C73F0B4238EB9_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass263_0_t9EA15DBB375982C7857A45FFC12C73F0B4238EB9* V_0 = NULL;
	delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* V_1 = NULL;
	{
		U3CU3Ec__DisplayClass263_0_t9EA15DBB375982C7857A45FFC12C73F0B4238EB9* L_0 = (U3CU3Ec__DisplayClass263_0_t9EA15DBB375982C7857A45FFC12C73F0B4238EB9*)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass263_0_t9EA15DBB375982C7857A45FFC12C73F0B4238EB9_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass263_0__ctor_mA0C4F9F9AEE43DFA9511F900E58F672A0208F64A(L_0, NULL);
		V_0 = L_0;
		U3CU3Ec__DisplayClass263_0_t9EA15DBB375982C7857A45FFC12C73F0B4238EB9* L_1 = V_0;
		strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* L_2 = ___0_f;
		NullCheck(L_1);
		L_1->___f = L_2;
		Il2CppCodeGenWriteBarrier((void**)(&L_1->___f), (void*)L_2);
		U3CU3Ec__DisplayClass263_0_t9EA15DBB375982C7857A45FFC12C73F0B4238EB9* L_3 = V_0;
		NullCheck(L_3);
		strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* L_4 = L_3->___f;
		if (L_4)
		{
			goto IL_0019;
		}
	}
	{
		V_1 = (delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712*)NULL;
		goto IL_0026;
	}

IL_0019:
	{
		U3CU3Ec__DisplayClass263_0_t9EA15DBB375982C7857A45FFC12C73F0B4238EB9* L_5 = V_0;
		delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* L_6 = (delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712*)il2cpp_codegen_object_new(delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712_il2cpp_TypeInfo_var);
		delegate_log__ctor_m90FE70F302D363779BDD720CE99A7B2A6B07E02A(L_6, L_5, (intptr_t)((void*)U3CU3Ec__DisplayClass263_0_U3Csqlite3_config_logU3Eb__0_m8B4286DB43D892CCAC433BFC538CE4D0117CC508_RuntimeMethod_var), NULL);
		V_1 = L_6;
	}

IL_0026:
	{
		delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* L_7 = V_1;
		RuntimeObject* L_8 = ___1_v;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_9;
		L_9 = raw_sqlite3_config_log_m57092048D3F7BE294DC58D14B05B6737DB1CCF94(L_7, L_8, NULL);
		return L_9;
	}
}
// Method Definition Index: 117261
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_log_m4A7A540937C79BDE846D0A5A574D264F880B88D0 (int32_t ___0_errcode, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_s, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		int32_t L_1 = ___0_errcode;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___1_s;
		NullCheck(L_0);
		InterfaceActionInvoker2< int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(84, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return;
	}
}
// Method Definition Index: 117262
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_log_m80A662212FADB1F0D2B4F5C63B3B88A831A1DEE5 (int32_t ___0_errcode, String_t* ___1_s, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		int32_t L_0 = ___0_errcode;
		String_t* L_1 = ___1_s;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		L_2 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_1, NULL);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		raw_sqlite3_log_m4A7A540937C79BDE846D0A5A574D264F880B88D0(L_0, L_2, NULL);
		return;
	}
}
// Method Definition Index: 117263
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_commit_hook_mCC7AD59656A32C8EC777F55EAA36CEA49C0E1F59 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* ___1_f, RuntimeObject* ___2_v, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* L_2 = ___1_f;
		RuntimeObject* L_3 = ___2_v;
		NullCheck(L_0);
		InterfaceActionInvoker3< sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A*, RuntimeObject* >::Invoke(85, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117264
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_rollback_hook_mD73E0F16766FEC8FC8BF99FD0EF4FD99757EC307 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* ___1_f, RuntimeObject* ___2_v, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* L_2 = ___1_f;
		RuntimeObject* L_3 = ___2_v;
		NullCheck(L_0);
		InterfaceActionInvoker3< sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174*, RuntimeObject* >::Invoke(86, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117265
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_trace_m50F2CA43FB6898436D6A2951D781295D390F19C3 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* ___1_f, RuntimeObject* ___2_v, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* L_2 = ___1_f;
		RuntimeObject* L_3 = ___2_v;
		NullCheck(L_0);
		InterfaceActionInvoker3< sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172*, RuntimeObject* >::Invoke(87, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117266
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_trace_mBD51D0E4060147639A421C15ACD0CD1CE55F28B8 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* ___1_f, RuntimeObject* ___2_v, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass269_0_U3Csqlite3_traceU3Eb__0_m3D7C6C768B76831B199A3DEEB4508F55616E75EC_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF* V_0 = NULL;
	delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* V_1 = NULL;
	{
		U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF* L_0 = (U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF*)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass269_0__ctor_m2C09696A2953271F2CEB026027FB892C3EF47178(L_0, NULL);
		V_0 = L_0;
		U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF* L_1 = V_0;
		strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* L_2 = ___1_f;
		NullCheck(L_1);
		L_1->___f = L_2;
		Il2CppCodeGenWriteBarrier((void**)(&L_1->___f), (void*)L_2);
		U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF* L_3 = V_0;
		RuntimeObject* L_4 = ___2_v;
		NullCheck(L_3);
		L_3->___v = L_4;
		Il2CppCodeGenWriteBarrier((void**)(&L_3->___v), (void*)L_4);
		U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF* L_5 = V_0;
		NullCheck(L_5);
		strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* L_6 = L_5->___f;
		if (L_6)
		{
			goto IL_0020;
		}
	}
	{
		V_1 = (delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172*)NULL;
		goto IL_002d;
	}

IL_0020:
	{
		U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF* L_7 = V_0;
		delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* L_8 = (delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172*)il2cpp_codegen_object_new(delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172_il2cpp_TypeInfo_var);
		delegate_trace__ctor_m363F86F59BCE053BF974338A6E05C69C95C8D189(L_8, L_7, (intptr_t)((void*)U3CU3Ec__DisplayClass269_0_U3Csqlite3_traceU3Eb__0_m3D7C6C768B76831B199A3DEEB4508F55616E75EC_RuntimeMethod_var), NULL);
		V_1 = L_8;
	}

IL_002d:
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_9 = ___0_db;
		delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* L_10 = V_1;
		U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF* L_11 = V_0;
		NullCheck(L_11);
		RuntimeObject* L_12 = L_11->___v;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		raw_sqlite3_trace_m50F2CA43FB6898436D6A2951D781295D390F19C3(L_9, L_10, L_12, NULL);
		return;
	}
}
// Method Definition Index: 117267
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_profile_m6C48BE4D8091B47292E4B1EC9CE06787430516A7 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* ___1_f, RuntimeObject* ___2_v, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* L_2 = ___1_f;
		RuntimeObject* L_3 = ___2_v;
		NullCheck(L_0);
		InterfaceActionInvoker3< sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779*, RuntimeObject* >::Invoke(88, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117268
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_profile_m648F79D1734AB61CE9253ED4739DC605A5D7FCE6 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* ___1_f, RuntimeObject* ___2_v, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass271_0_U3Csqlite3_profileU3Eb__0_mC0125502FE73FA12ED8241058AA31E17A35BD0C7_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F* V_0 = NULL;
	delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* V_1 = NULL;
	{
		U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F* L_0 = (U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F*)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass271_0__ctor_mD621FC47AF62F2940A6152CDA5BA7F8FC5CE324C(L_0, NULL);
		V_0 = L_0;
		U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F* L_1 = V_0;
		strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* L_2 = ___1_f;
		NullCheck(L_1);
		L_1->___f = L_2;
		Il2CppCodeGenWriteBarrier((void**)(&L_1->___f), (void*)L_2);
		U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F* L_3 = V_0;
		RuntimeObject* L_4 = ___2_v;
		NullCheck(L_3);
		L_3->___v = L_4;
		Il2CppCodeGenWriteBarrier((void**)(&L_3->___v), (void*)L_4);
		U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F* L_5 = V_0;
		NullCheck(L_5);
		strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* L_6 = L_5->___f;
		if (L_6)
		{
			goto IL_0020;
		}
	}
	{
		V_1 = (delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779*)NULL;
		goto IL_002d;
	}

IL_0020:
	{
		U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F* L_7 = V_0;
		delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* L_8 = (delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779*)il2cpp_codegen_object_new(delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779_il2cpp_TypeInfo_var);
		delegate_profile__ctor_mACA8FE55EFC53A98950C22BD55D8B8C0A3F6CEDA(L_8, L_7, (intptr_t)((void*)U3CU3Ec__DisplayClass271_0_U3Csqlite3_profileU3Eb__0_mC0125502FE73FA12ED8241058AA31E17A35BD0C7_RuntimeMethod_var), NULL);
		V_1 = L_8;
	}

IL_002d:
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_9 = ___0_db;
		delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* L_10 = V_1;
		U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F* L_11 = V_0;
		NullCheck(L_11);
		RuntimeObject* L_12 = L_11->___v;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		raw_sqlite3_profile_m6C48BE4D8091B47292E4B1EC9CE06787430516A7(L_9, L_10, L_12, NULL);
		return;
	}
}
// Method Definition Index: 117269
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_progress_handler_mC25CF5C7797DD184B69993C1B6BDC16F1D4E5C5D (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, int32_t ___1_instructions, delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* ___2_func, RuntimeObject* ___3_v, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		int32_t L_2 = ___1_instructions;
		delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* L_3 = ___2_func;
		RuntimeObject* L_4 = ___3_v;
		NullCheck(L_0);
		InterfaceActionInvoker4< sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, int32_t, delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710*, RuntimeObject* >::Invoke(89, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3, L_4);
		return;
	}
}
// Method Definition Index: 117270
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_update_hook_mCBD72FAA0BFA925F97C3230A3FA20A51A2AAD719 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* ___1_f, RuntimeObject* ___2_v, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* L_2 = ___1_f;
		RuntimeObject* L_3 = ___2_v;
		NullCheck(L_0);
		InterfaceActionInvoker3< sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80*, RuntimeObject* >::Invoke(90, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117271
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_update_hook_m7702F10863D912D3E3E1E6BD04008180B5BAC4D9 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* ___1_f, RuntimeObject* ___2_v, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass274_0_U3Csqlite3_update_hookU3Eb__0_m9A7EB3C9B70FD4005DD270F143CDE0D73F91C033_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass274_0_t75B1EC8A8B2CD9CFE76F0CDD3507D686F9C95A31_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass274_0_t75B1EC8A8B2CD9CFE76F0CDD3507D686F9C95A31* V_0 = NULL;
	delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* V_1 = NULL;
	{
		U3CU3Ec__DisplayClass274_0_t75B1EC8A8B2CD9CFE76F0CDD3507D686F9C95A31* L_0 = (U3CU3Ec__DisplayClass274_0_t75B1EC8A8B2CD9CFE76F0CDD3507D686F9C95A31*)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass274_0_t75B1EC8A8B2CD9CFE76F0CDD3507D686F9C95A31_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass274_0__ctor_m6B059B3B06DFD76E37692FC3BCC6DAA722C29C3C(L_0, NULL);
		V_0 = L_0;
		U3CU3Ec__DisplayClass274_0_t75B1EC8A8B2CD9CFE76F0CDD3507D686F9C95A31* L_1 = V_0;
		strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* L_2 = ___1_f;
		NullCheck(L_1);
		L_1->___f = L_2;
		Il2CppCodeGenWriteBarrier((void**)(&L_1->___f), (void*)L_2);
		U3CU3Ec__DisplayClass274_0_t75B1EC8A8B2CD9CFE76F0CDD3507D686F9C95A31* L_3 = V_0;
		NullCheck(L_3);
		strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* L_4 = L_3->___f;
		if (L_4)
		{
			goto IL_0019;
		}
	}
	{
		V_1 = (delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80*)NULL;
		goto IL_0026;
	}

IL_0019:
	{
		U3CU3Ec__DisplayClass274_0_t75B1EC8A8B2CD9CFE76F0CDD3507D686F9C95A31* L_5 = V_0;
		delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* L_6 = (delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80*)il2cpp_codegen_object_new(delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80_il2cpp_TypeInfo_var);
		delegate_update__ctor_m17032A8DE0B56F6F7EEB38252AA6FF345689EE93(L_6, L_5, (intptr_t)((void*)U3CU3Ec__DisplayClass274_0_U3Csqlite3_update_hookU3Eb__0_m9A7EB3C9B70FD4005DD270F143CDE0D73F91C033_RuntimeMethod_var), NULL);
		V_1 = L_6;
	}

IL_0026:
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_7 = ___0_db;
		delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* L_8 = V_1;
		RuntimeObject* L_9 = ___2_v;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		raw_sqlite3_update_hook_mCBD72FAA0BFA925F97C3230A3FA20A51A2AAD719(L_7, L_8, L_9, NULL);
		return;
	}
}
// Method Definition Index: 117272
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_create_collation_m38533236347D44E43728C7B27FA8F457ABC60AB6 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_name, RuntimeObject* ___2_v, strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* ___3_f, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass275_0_U3Csqlite3_create_collationU3Eb__0_m04DCDC3168B2FE4CB267E97F21757904478F22F4_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass275_0_t2E8E6521F4006ACCD7E5F26205A9BC7CBA6A4B9A_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass275_0_t2E8E6521F4006ACCD7E5F26205A9BC7CBA6A4B9A* V_0 = NULL;
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* V_1 = NULL;
	delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* V_2 = NULL;
	{
		U3CU3Ec__DisplayClass275_0_t2E8E6521F4006ACCD7E5F26205A9BC7CBA6A4B9A* L_0 = (U3CU3Ec__DisplayClass275_0_t2E8E6521F4006ACCD7E5F26205A9BC7CBA6A4B9A*)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass275_0_t2E8E6521F4006ACCD7E5F26205A9BC7CBA6A4B9A_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass275_0__ctor_mE3C57C5B8020F19D1D8240BBF41CC41C413E7838(L_0, NULL);
		V_0 = L_0;
		U3CU3Ec__DisplayClass275_0_t2E8E6521F4006ACCD7E5F26205A9BC7CBA6A4B9A* L_1 = V_0;
		strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* L_2 = ___3_f;
		NullCheck(L_1);
		L_1->___f = L_2;
		Il2CppCodeGenWriteBarrier((void**)(&L_1->___f), (void*)L_2);
		String_t* L_3 = ___1_name;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_4;
		L_4 = util_to_utf8_with_z_m3C8DD1FCA689A8E3D7C8AD391BAAFCAC45F6596E(L_3, NULL);
		V_1 = L_4;
		U3CU3Ec__DisplayClass275_0_t2E8E6521F4006ACCD7E5F26205A9BC7CBA6A4B9A* L_5 = V_0;
		NullCheck(L_5);
		strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* L_6 = L_5->___f;
		if (L_6)
		{
			goto IL_0020;
		}
	}
	{
		V_2 = (delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF*)NULL;
		goto IL_002d;
	}

IL_0020:
	{
		U3CU3Ec__DisplayClass275_0_t2E8E6521F4006ACCD7E5F26205A9BC7CBA6A4B9A* L_7 = V_0;
		delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* L_8 = (delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF*)il2cpp_codegen_object_new(delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF_il2cpp_TypeInfo_var);
		delegate_collation__ctor_mF884888642B63BD696BC37E898161379BDAA85AB(L_8, L_7, (intptr_t)((void*)U3CU3Ec__DisplayClass275_0_U3Csqlite3_create_collationU3Eb__0_m04DCDC3168B2FE4CB267E97F21757904478F22F4_RuntimeMethod_var), NULL);
		V_2 = L_8;
	}

IL_002d:
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_9;
		L_9 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_10 = ___0_db;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_11 = V_1;
		RuntimeObject* L_12 = ___2_v;
		delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* L_13 = V_2;
		NullCheck(L_9);
		int32_t L_14;
		L_14 = InterfaceFuncInvoker4< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*, RuntimeObject*, delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* >::Invoke(91, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_9, L_10, L_11, L_12, L_13);
		return L_14;
	}
}
// Method Definition Index: 117273
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3__create_collation_utf8_m70D4F751BA7685E8B1B75685B0F56126D5EE51A4 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_name, RuntimeObject* ___2_v, delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* ___3_f, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* V_0 = NULL;
	{
		String_t* L_0 = ___1_name;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_1;
		L_1 = util_to_utf8_with_z_m3C8DD1FCA689A8E3D7C8AD391BAAFCAC45F6596E(L_0, NULL);
		V_0 = L_1;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_2;
		L_2 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_3 = ___0_db;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_4 = V_0;
		RuntimeObject* L_5 = ___2_v;
		delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* L_6 = ___3_f;
		NullCheck(L_2);
		int32_t L_7;
		L_7 = InterfaceFuncInvoker4< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*, RuntimeObject*, delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* >::Invoke(91, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_2, L_3, L_4, L_5, L_6);
		return L_7;
	}
}
// Method Definition Index: 117274
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_create_function_mA7967B3FB878B8FF5420E578A3C312FB021826A0 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_name, int32_t ___2_nArg, int32_t ___3_flags, RuntimeObject* ___4_v, delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* ___5_func, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* V_0 = NULL;
	{
		String_t* L_0 = ___1_name;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_1;
		L_1 = util_to_utf8_with_z_m3C8DD1FCA689A8E3D7C8AD391BAAFCAC45F6596E(L_0, NULL);
		V_0 = L_1;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_2;
		L_2 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_3 = ___0_db;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_4 = V_0;
		int32_t L_5 = ___2_nArg;
		int32_t L_6 = ___3_flags;
		RuntimeObject* L_7 = ___4_v;
		delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* L_8 = ___5_func;
		NullCheck(L_2);
		int32_t L_9;
		L_9 = InterfaceFuncInvoker6< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*, int32_t, int32_t, RuntimeObject*, delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* >::Invoke(92, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_2, L_3, L_4, L_5, L_6, L_7, L_8);
		return L_9;
	}
}
// Method Definition Index: 117275
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_create_function_mCFEE0A69952688715C75CFAD8EC16D3FB773589A (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_name, int32_t ___2_nArg, int32_t ___3_flags, RuntimeObject* ___4_v, delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* ___5_func_step, delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* ___6_func_final, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* V_0 = NULL;
	{
		String_t* L_0 = ___1_name;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_1;
		L_1 = util_to_utf8_with_z_m3C8DD1FCA689A8E3D7C8AD391BAAFCAC45F6596E(L_0, NULL);
		V_0 = L_1;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_2;
		L_2 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_3 = ___0_db;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_4 = V_0;
		int32_t L_5 = ___2_nArg;
		int32_t L_6 = ___3_flags;
		RuntimeObject* L_7 = ___4_v;
		delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* L_8 = ___5_func_step;
		delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* L_9 = ___6_func_final;
		NullCheck(L_2);
		int32_t L_10;
		L_10 = InterfaceFuncInvoker7< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*, int32_t, int32_t, RuntimeObject*, delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957*, delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* >::Invoke(93, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_2, L_3, L_4, L_5, L_6, L_7, L_8, L_9);
		return L_10;
	}
}
// Method Definition Index: 117276
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_create_function_mBDE9C59FC3910597F9C5705309CC0558EEA786AC (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_name, int32_t ___2_nArg, RuntimeObject* ___3_v, delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* ___4_func, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_0 = ___0_db;
		String_t* L_1 = ___1_name;
		int32_t L_2 = ___2_nArg;
		RuntimeObject* L_3 = ___3_v;
		delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* L_4 = ___4_func;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_5;
		L_5 = raw_sqlite3_create_function_mA7967B3FB878B8FF5420E578A3C312FB021826A0(L_0, L_1, L_2, 0, L_3, L_4, NULL);
		return L_5;
	}
}
// Method Definition Index: 117277
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_create_function_m3DC8C75FA03E86488592EF3906EF21548B70B118 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_name, int32_t ___2_nArg, RuntimeObject* ___3_v, delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* ___4_func_step, delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* ___5_func_final, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_0 = ___0_db;
		String_t* L_1 = ___1_name;
		int32_t L_2 = ___2_nArg;
		RuntimeObject* L_3 = ___3_v;
		delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* L_4 = ___4_func_step;
		delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* L_5 = ___5_func_final;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_6;
		L_6 = raw_sqlite3_create_function_mCFEE0A69952688715C75CFAD8EC16D3FB773589A(L_0, L_1, L_2, 0, L_3, L_4, L_5, NULL);
		return L_6;
	}
}
// Method Definition Index: 117278
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_db_status_m7971C7C66FB59945BCD4AD224F53BD01448EAEE2 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, int32_t ___1_op, int32_t* ___2_current, int32_t* ___3_highest, int32_t ___4_resetFlg, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		int32_t L_2 = ___1_op;
		int32_t* L_3 = ___2_current;
		int32_t* L_4 = ___3_highest;
		int32_t L_5 = ___4_resetFlg;
		NullCheck(L_0);
		int32_t L_6;
		L_6 = InterfaceFuncInvoker5< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, int32_t, int32_t*, int32_t*, int32_t >::Invoke(94, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3, L_4, L_5);
		return L_6;
	}
}
// Method Definition Index: 117279
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* raw_utf8_span_to_string_m1D3AC8DF369A2FFC9AC7B1A7E165F6B4C7234DE2 (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_GetPinnableReference_m365106BC7655B6A45D47673D473A699B5B69DA57_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709);
		s_Il2CppMethodInitialized = true;
	}
	uint8_t* V_0 = NULL;
	uint8_t* V_1 = NULL;
	{
		int32_t L_0;
		L_0 = ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_inline((&___0_p), ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		if (L_0)
		{
			goto IL_000f;
		}
	}
	{
		return _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709;
	}

IL_000f:
	{
		uint8_t* L_1;
		L_1 = ReadOnlySpan_1_GetPinnableReference_m365106BC7655B6A45D47673D473A699B5B69DA57((&___0_p), ReadOnlySpan_1_GetPinnableReference_m365106BC7655B6A45D47673D473A699B5B69DA57_RuntimeMethod_var);
		V_1 = L_1;
		uint8_t* L_2 = V_1;
		V_0 = (uint8_t*)((uintptr_t)L_2);
		Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* L_3;
		L_3 = Encoding_get_UTF8_m9FA98A53CE96FD6D02982625C5246DD36C1235C9(NULL);
		uint8_t* L_4 = V_0;
		int32_t L_5;
		L_5 = ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_inline((&___0_p), ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		NullCheck(L_3);
		String_t* L_6;
		L_6 = Encoding_GetString_m42BFF0862341DCD5289A7D75B5D7A22CE9690EAD(L_3, L_4, L_5, NULL);
		return L_6;
	}
}
// Method Definition Index: 117280
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_key_m1320BCE70CDFAD61CBB71F0030BDDC86D941D35C (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_k, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_2 = ___1_k;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D >::Invoke(132, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117281
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_key_v2_mE8A95A10663EC81CF57563B2B984FC897B878296 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_name, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_k, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___1_name;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_3 = ___2_k;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D >::Invoke(133, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117282
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_rekey_m2CE906910A22160F1255DF061F408786F36D4C06 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_k, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_2 = ___1_k;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D >::Invoke(134, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117283
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_rekey_v2_m6461C0D5475AB6ED9771E7F4C72C4A041D11C22C (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_name, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_k, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___1_name;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_3 = ___2_k;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D >::Invoke(135, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117284
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_libversion_m6C96593669D42355209A9F6544E91BB5E44164C8 (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_1;
		L_1 = InterfaceFuncInvoker0< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(9, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0);
		return L_1;
	}
}
// Method Definition Index: 117285
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_libversion_number_m74065339EAB352F4CCEE2B6C1BDDF823E3C47CA1 (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		NullCheck(L_0);
		int32_t L_1;
		L_1 = InterfaceFuncInvoker0< int32_t >::Invoke(10, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0);
		return L_1;
	}
}
// Method Definition Index: 117286
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_threadsafe_mA1EA15B96BA19C04C9D827D9CEEBBC041264773E (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		NullCheck(L_0);
		int32_t L_1;
		L_1 = InterfaceFuncInvoker0< int32_t >::Invoke(8, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0);
		return L_1;
	}
}
// Method Definition Index: 117287
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_initialize_m6F068A1E9DC50558A53FCBE1AE7C33AD966AB620 (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		NullCheck(L_0);
		int32_t L_1;
		L_1 = InterfaceFuncInvoker0< int32_t >::Invoke(137, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0);
		return L_1;
	}
}
// Method Definition Index: 117288
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_shutdown_m2ECBFE4B21C59A8FE52761840A6BD62EAA1AC5D6 (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		NullCheck(L_0);
		int32_t L_1;
		L_1 = InterfaceFuncInvoker0< int32_t >::Invoke(138, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0);
		return L_1;
	}
}
// Method Definition Index: 117289
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_limit_mE85EDCD7393EAB2460105DA3A40ED6F621291772 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, int32_t ___1_id, int32_t ___2_newVal, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		int32_t L_2 = ___1_id;
		int32_t L_3 = ___2_newVal;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, int32_t, int32_t >::Invoke(139, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117290
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_config_mB2BC16E297F89709654A3947322ADC4F5D2ADF4E (int32_t ___0_op, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		int32_t L_1 = ___0_op;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, int32_t >::Invoke(140, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117291
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_config_m9861AFFD3FA3C9D0E84AB37C4E54A6C1AE71726D (int32_t ___0_op, int32_t ___1_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		int32_t L_1 = ___0_op;
		int32_t L_2 = ___1_val;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, int32_t, int32_t >::Invoke(141, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117292
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_db_config_mB6E0C2E29DE03A0F3447D341B5A7F5307253EA74 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, int32_t ___1_op, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		int32_t L_2 = ___1_op;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3 = ___2_val;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(142, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117293
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_db_config_m178F1DD55F01E18B4CE4F3920602879F7862995A (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, int32_t ___1_op, int32_t ___2_val, int32_t* ___3_result, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		int32_t L_2 = ___1_op;
		int32_t L_3 = ___2_val;
		int32_t* L_4 = ___3_result;
		NullCheck(L_0);
		int32_t L_5;
		L_5 = InterfaceFuncInvoker4< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, int32_t, int32_t, int32_t* >::Invoke(143, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3, L_4);
		return L_5;
	}
}
// Method Definition Index: 117294
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_db_config_m3130746108353E30411C1F8FF7C732E837F0D73C (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, int32_t ___1_op, intptr_t ___2_ptr, int32_t ___3_int0, int32_t ___4_int1, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		int32_t L_2 = ___1_op;
		intptr_t L_3 = ___2_ptr;
		int32_t L_4 = ___3_int0;
		int32_t L_5 = ___4_int1;
		NullCheck(L_0);
		int32_t L_6;
		L_6 = InterfaceFuncInvoker5< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, int32_t, intptr_t, int32_t, int32_t >::Invoke(144, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3, L_4, L_5);
		return L_6;
	}
}
// Method Definition Index: 117295
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_enable_load_extension_m3149E233D93EE67B624ADD52B517CA6DB806EE7E (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, int32_t ___1_onoff, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		int32_t L_2 = ___1_onoff;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, int32_t >::Invoke(145, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117296
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_load_extension_m96FF3455FBFB22B1DC60CD87A1423674D0ACBD64 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_file, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_proc, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* ___3_errmsg, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___1_file;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3 = ___2_proc;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* L_4 = ___3_errmsg;
		NullCheck(L_0);
		int32_t L_5;
		L_5 = InterfaceFuncInvoker4< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* >::Invoke(136, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3, L_4);
		return L_5;
	}
}
// Method Definition Index: 117297
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_sourceid_mE7911E8DD63779B322B9CC13645FA233FE5A7AC6 (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_1;
		L_1 = InterfaceFuncInvoker0< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(11, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0);
		return L_1;
	}
}
// Method Definition Index: 117298
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t raw_sqlite3_memory_used_m06B6655B81E6733D604060B6F3D486792350243B (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		NullCheck(L_0);
		int64_t L_1;
		L_1 = InterfaceFuncInvoker0< int64_t >::Invoke(12, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0);
		return L_1;
	}
}
// Method Definition Index: 117299
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t raw_sqlite3_memory_highwater_m721A910BFE1ADE369565AAC47BD7F8B08AE4FD6E (int32_t ___0_resetFlag, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		int32_t L_1 = ___0_resetFlag;
		NullCheck(L_0);
		int64_t L_2;
		L_2 = InterfaceFuncInvoker1< int64_t, int32_t >::Invoke(13, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117300
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t raw_sqlite3_soft_heap_limit64_mF9584F2465A06F8F0E07016A4D7A10350B3DB254 (int64_t ___0_n, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		int64_t L_1 = ___0_n;
		NullCheck(L_0);
		int64_t L_2;
		L_2 = InterfaceFuncInvoker1< int64_t, int64_t >::Invoke(14, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117301
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t raw_sqlite3_hard_heap_limit64_m688CB2ADFC1095CD6326085716A1F0E5A8F48C9C (int64_t ___0_n, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		int64_t L_1 = ___0_n;
		NullCheck(L_0);
		int64_t L_2;
		L_2 = InterfaceFuncInvoker1< int64_t, int64_t >::Invoke(15, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117302
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_status_m4519DCD6559D19087DD4DF01C4A5DE4514D09FAB (int32_t ___0_op, int32_t* ___1_current, int32_t* ___2_highwater, int32_t ___3_resetFlag, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		int32_t L_1 = ___0_op;
		int32_t* L_2 = ___1_current;
		int32_t* L_3 = ___2_highwater;
		int32_t L_4 = ___3_resetFlag;
		NullCheck(L_0);
		int32_t L_5;
		L_5 = InterfaceFuncInvoker4< int32_t, int32_t, int32_t*, int32_t*, int32_t >::Invoke(16, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3, L_4);
		return L_5;
	}
}
// Method Definition Index: 117303
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_errmsg_mFE03CEF0386CDA91880F0E8EAC525C22D450A19E (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		L_2 = InterfaceFuncInvoker1< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* >::Invoke(19, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117304
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_db_readonly_m628A7A82CB13F774B40F2D3250710453F6FF5462 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_dbName, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___1_dbName;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(17, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117305
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_db_readonly_mE1C2389B66B78333C74B5407E5CBA7B94469A170 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_dbName, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_0 = ___0_db;
		String_t* L_1 = ___1_dbName;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		L_2 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_1, NULL);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_3;
		L_3 = raw_sqlite3_db_readonly_m628A7A82CB13F774B40F2D3250710453F6FF5462(L_0, L_2, NULL);
		return L_3;
	}
}
// Method Definition Index: 117306
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_db_filename_m057E3BE2DA4DF24BCFAEE1B7D361FB4D81D00CA9 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_att, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___1_att;
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = InterfaceFuncInvoker2< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(18, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117307
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_db_filename_mBA8A954B3A9B88D36EB8EF9309B483B83D6A5457 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_att, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_0 = ___0_db;
		String_t* L_1 = ___1_att;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		L_2 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_1, NULL);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = raw_sqlite3_db_filename_m057E3BE2DA4DF24BCFAEE1B7D361FB4D81D00CA9(L_0, L_2, NULL);
		return L_3;
	}
}
// Method Definition Index: 117308
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t raw_sqlite3_last_insert_rowid_m3AFE748672D452D1ED4FF65802095A0829D30D35 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		NullCheck(L_0);
		int64_t L_2;
		L_2 = InterfaceFuncInvoker1< int64_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* >::Invoke(20, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117309
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_changes_m9E3C3BADDD66190E5C41C164A452D2729719C01E (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* >::Invoke(21, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117310
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_total_changes_m62EC9B6E04FD21AC63FD70F3AE1574C3358B5ABA (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* >::Invoke(22, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117311
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_get_autocommit_mB5BEEFD588FE21EF2D6C59862EDCA05B039E2ABC (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* >::Invoke(23, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117312
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_busy_timeout_m3F2D77C3B8C25B213B17B87F62DBD2A27947E7C8 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, int32_t ___1_ms, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		int32_t L_2 = ___1_ms;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, int32_t >::Invoke(24, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117313
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_extended_result_codes_mD06290166E8EF358D1561EC7731A37C7950ABE83 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, int32_t ___1_onoff, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		int32_t L_2 = ___1_onoff;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, int32_t >::Invoke(25, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117314
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_errcode_m747E9482474F3689F7AD9B5163925B3B4681D014 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* >::Invoke(26, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117315
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_extended_errcode_m232A4B5C8515EACBABC14726063FD1C1A568E04A (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* >::Invoke(27, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117316
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_errstr_m79F209ECD49642D129CFBBF309EE2563F7BAB5BA (int32_t ___0_rc, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		int32_t L_1 = ___0_rc;
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		L_2 = InterfaceFuncInvoker1< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int32_t >::Invoke(28, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117317
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_prepare_v2_mE1DE95E0857D275D9B7AB6DD883BD058EB6A380A (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_sql, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___2_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_2 = ___1_sql;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker4< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, intptr_t*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* >::Invoke(29, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, (&V_0), (&V_1));
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** L_4 = ___2_stmt;
		intptr_t L_5 = V_0;
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_6 = ___0_db;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_7;
		L_7 = sqlite3_stmt_From_mA7A4E31058B72E4386C4B74F4B1A93BF446CC207(L_5, L_6, NULL);
		*((sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_4) = (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_7;
		Il2CppCodeGenWriteBarrier((void**)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_4, (void*)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_7);
		return L_3;
	}
}
// Method Definition Index: 117318
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_prepare_v2_m8523654E5B893A4BDD28F736343E588BC7B69089 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_sql, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___2_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___1_sql;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker4< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, intptr_t*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* >::Invoke(31, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, (&V_0), (&V_1));
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** L_4 = ___2_stmt;
		intptr_t L_5 = V_0;
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_6 = ___0_db;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_7;
		L_7 = sqlite3_stmt_From_mA7A4E31058B72E4386C4B74F4B1A93BF446CC207(L_5, L_6, NULL);
		*((sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_4) = (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_7;
		Il2CppCodeGenWriteBarrier((void**)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_4, (void*)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_7);
		return L_3;
	}
}
// Method Definition Index: 117319
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_prepare_v2_m7849E3B108C1ECB02F3C546CCE21D3B94F0A6D2E (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_sql, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___2_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* V_0 = NULL;
	ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D V_1;
	memset((&V_1), 0, sizeof(V_1));
	ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D V_2;
	memset((&V_2), 0, sizeof(V_2));
	{
		String_t* L_0 = ___1_sql;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_1;
		L_1 = util_to_utf8_with_z_m3C8DD1FCA689A8E3D7C8AD391BAAFCAC45F6596E(L_0, NULL);
		V_0 = L_1;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_2 = V_0;
		ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_inline((&V_1), L_2, ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_RuntimeMethod_var);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_3 = ___0_db;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_4 = V_1;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** L_5 = ___2_stmt;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_6;
		L_6 = raw_sqlite3_prepare_v2_m88597C248DBAC08DBD376C3432FD9250607D4ACB(L_3, L_4, L_5, (&V_2), NULL);
		return L_6;
	}
}
// Method Definition Index: 117320
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_prepare_v2_m88597C248DBAC08DBD376C3432FD9250607D4ACB (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_sql, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___2_stmt, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* ___3_tail, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_2 = ___1_sql;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* L_3 = ___3_tail;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker4< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, intptr_t*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* >::Invoke(29, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, (&V_0), L_3);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** L_5 = ___2_stmt;
		intptr_t L_6 = V_0;
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_7 = ___0_db;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_8;
		L_8 = sqlite3_stmt_From_mA7A4E31058B72E4386C4B74F4B1A93BF446CC207(L_6, L_7, NULL);
		*((sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_5) = (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_8;
		Il2CppCodeGenWriteBarrier((void**)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_5, (void*)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_8);
		return L_4;
	}
}
// Method Definition Index: 117321
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_prepare_v2_mE6FD2D5EEF9D9CB7CE549FE3D0CAAA6D9F207DE1 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_sql, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___2_stmt, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* ___3_tail, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___1_sql;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* L_3 = ___3_tail;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker4< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, intptr_t*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* >::Invoke(31, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, (&V_0), L_3);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** L_5 = ___2_stmt;
		intptr_t L_6 = V_0;
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_7 = ___0_db;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_8;
		L_8 = sqlite3_stmt_From_mA7A4E31058B72E4386C4B74F4B1A93BF446CC207(L_6, L_7, NULL);
		*((sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_5) = (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_8;
		Il2CppCodeGenWriteBarrier((void**)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_5, (void*)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_8);
		return L_4;
	}
}
// Method Definition Index: 117322
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_prepare_v2_m0CC63D0604B8A2CFA0BB4B95A0A73136E0598942 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_sql, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___2_stmt, String_t** ___3_tail, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_Slice_mEB3D3A427170FC5A0AB734619D4792C299697C89_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* V_0 = NULL;
	ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D V_1;
	memset((&V_1), 0, sizeof(V_1));
	ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D V_2;
	memset((&V_2), 0, sizeof(V_2));
	String_t** G_B2_0 = NULL;
	int32_t G_B2_1 = 0;
	String_t** G_B1_0 = NULL;
	int32_t G_B1_1 = 0;
	String_t* G_B3_0 = NULL;
	String_t** G_B3_1 = NULL;
	int32_t G_B3_2 = 0;
	{
		String_t* L_0 = ___1_sql;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_1;
		L_1 = util_to_utf8_with_z_m3C8DD1FCA689A8E3D7C8AD391BAAFCAC45F6596E(L_0, NULL);
		V_0 = L_1;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_2 = V_0;
		ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_inline((&V_1), L_2, ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_RuntimeMethod_var);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_3 = ___0_db;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_4 = V_1;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** L_5 = ___2_stmt;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_6;
		L_6 = raw_sqlite3_prepare_v2_m88597C248DBAC08DBD376C3432FD9250607D4ACB(L_3, L_4, L_5, (&V_2), NULL);
		String_t** L_7 = ___3_tail;
		int32_t L_8;
		L_8 = ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_inline((&V_2), ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		if ((((int32_t)L_8) > ((int32_t)0)))
		{
			G_B2_0 = L_7;
			G_B2_1 = L_6;
			goto IL_002b;
		}
		G_B1_0 = L_7;
		G_B1_1 = L_6;
	}
	{
		G_B3_0 = _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709;
		G_B3_1 = G_B1_0;
		G_B3_2 = G_B1_1;
		goto IL_0041;
	}

IL_002b:
	{
		int32_t L_9;
		L_9 = ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_inline((&V_2), ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_10;
		L_10 = ReadOnlySpan_1_Slice_mEB3D3A427170FC5A0AB734619D4792C299697C89_inline((&V_2), 0, ((int32_t)il2cpp_codegen_subtract(L_9, 1)), ReadOnlySpan_1_Slice_mEB3D3A427170FC5A0AB734619D4792C299697C89_RuntimeMethod_var);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		String_t* L_11;
		L_11 = raw_utf8_span_to_string_m1D3AC8DF369A2FFC9AC7B1A7E165F6B4C7234DE2(L_10, NULL);
		G_B3_0 = L_11;
		G_B3_1 = G_B2_0;
		G_B3_2 = G_B2_1;
	}

IL_0041:
	{
		*((String_t**)G_B3_1) = (String_t*)G_B3_0;
		Il2CppCodeGenWriteBarrier((void**)(String_t**)G_B3_1, (void*)(String_t*)G_B3_0);
		return G_B3_2;
	}
}
// Method Definition Index: 117323
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_prepare_v3_m5A230A1555FE2A14DA23A342C927E94098C7E2E0 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_sql, uint32_t ___2_flags, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___3_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_2 = ___1_sql;
		uint32_t L_3 = ___2_flags;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker5< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, uint32_t, intptr_t*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* >::Invoke(30, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3, (&V_0), (&V_1));
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** L_5 = ___3_stmt;
		intptr_t L_6 = V_0;
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_7 = ___0_db;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_8;
		L_8 = sqlite3_stmt_From_mA7A4E31058B72E4386C4B74F4B1A93BF446CC207(L_6, L_7, NULL);
		*((sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_5) = (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_8;
		Il2CppCodeGenWriteBarrier((void**)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_5, (void*)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_8);
		return L_4;
	}
}
// Method Definition Index: 117324
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_prepare_v3_mDB52D55A0BBCD4D8DB3088D1245D10BD028D0D1A (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_sql, uint32_t ___2_flags, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___3_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___1_sql;
		uint32_t L_3 = ___2_flags;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker5< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, uint32_t, intptr_t*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* >::Invoke(32, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3, (&V_0), (&V_1));
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** L_5 = ___3_stmt;
		intptr_t L_6 = V_0;
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_7 = ___0_db;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_8;
		L_8 = sqlite3_stmt_From_mA7A4E31058B72E4386C4B74F4B1A93BF446CC207(L_6, L_7, NULL);
		*((sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_5) = (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_8;
		Il2CppCodeGenWriteBarrier((void**)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_5, (void*)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_8);
		return L_4;
	}
}
// Method Definition Index: 117325
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_prepare_v3_mDAB3475BCCCCD34FCB6DFF48F3F9B7497B44FDB2 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_sql, uint32_t ___2_flags, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___3_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* V_0 = NULL;
	ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D V_1;
	memset((&V_1), 0, sizeof(V_1));
	ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D V_2;
	memset((&V_2), 0, sizeof(V_2));
	{
		String_t* L_0 = ___1_sql;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_1;
		L_1 = util_to_utf8_with_z_m3C8DD1FCA689A8E3D7C8AD391BAAFCAC45F6596E(L_0, NULL);
		V_0 = L_1;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_2 = V_0;
		ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_inline((&V_1), L_2, ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_RuntimeMethod_var);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_3 = ___0_db;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_4 = V_1;
		uint32_t L_5 = ___2_flags;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** L_6 = ___3_stmt;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_7;
		L_7 = raw_sqlite3_prepare_v3_m5F2871468C2CC403C7DF6EFFD75BACA36C6FE92F(L_3, L_4, L_5, L_6, (&V_2), NULL);
		return L_7;
	}
}
// Method Definition Index: 117326
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_prepare_v3_m5F2871468C2CC403C7DF6EFFD75BACA36C6FE92F (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_sql, uint32_t ___2_flags, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___3_stmt, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* ___4_tail, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_2 = ___1_sql;
		uint32_t L_3 = ___2_flags;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* L_4 = ___4_tail;
		NullCheck(L_0);
		int32_t L_5;
		L_5 = InterfaceFuncInvoker5< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, uint32_t, intptr_t*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* >::Invoke(30, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3, (&V_0), L_4);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** L_6 = ___3_stmt;
		intptr_t L_7 = V_0;
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_8 = ___0_db;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_9;
		L_9 = sqlite3_stmt_From_mA7A4E31058B72E4386C4B74F4B1A93BF446CC207(L_7, L_8, NULL);
		*((sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_6) = (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_9;
		Il2CppCodeGenWriteBarrier((void**)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_6, (void*)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_9);
		return L_5;
	}
}
// Method Definition Index: 117327
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_prepare_v3_m76FC45FBF99D18FBD6B916B452290988BB00CB7C (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_sql, uint32_t ___2_flags, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___3_stmt, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* ___4_tail, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___1_sql;
		uint32_t L_3 = ___2_flags;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* L_4 = ___4_tail;
		NullCheck(L_0);
		int32_t L_5;
		L_5 = InterfaceFuncInvoker5< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, uint32_t, intptr_t*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* >::Invoke(32, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3, (&V_0), L_4);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** L_6 = ___3_stmt;
		intptr_t L_7 = V_0;
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_8 = ___0_db;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_9;
		L_9 = sqlite3_stmt_From_mA7A4E31058B72E4386C4B74F4B1A93BF446CC207(L_7, L_8, NULL);
		*((sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_6) = (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_9;
		Il2CppCodeGenWriteBarrier((void**)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F**)L_6, (void*)(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)L_9);
		return L_5;
	}
}
// Method Definition Index: 117328
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_prepare_v3_mCA250883482520CD7BBDE0193981ED660BB95A8C (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_sql, uint32_t ___2_flags, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** ___3_stmt, String_t** ___4_tail, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_Slice_mEB3D3A427170FC5A0AB734619D4792C299697C89_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* V_0 = NULL;
	ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D V_1;
	memset((&V_1), 0, sizeof(V_1));
	ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D V_2;
	memset((&V_2), 0, sizeof(V_2));
	String_t** G_B2_0 = NULL;
	int32_t G_B2_1 = 0;
	String_t** G_B1_0 = NULL;
	int32_t G_B1_1 = 0;
	String_t* G_B3_0 = NULL;
	String_t** G_B3_1 = NULL;
	int32_t G_B3_2 = 0;
	{
		String_t* L_0 = ___1_sql;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_1;
		L_1 = util_to_utf8_with_z_m3C8DD1FCA689A8E3D7C8AD391BAAFCAC45F6596E(L_0, NULL);
		V_0 = L_1;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_2 = V_0;
		ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_inline((&V_1), L_2, ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_RuntimeMethod_var);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_3 = ___0_db;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_4 = V_1;
		uint32_t L_5 = ___2_flags;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F** L_6 = ___3_stmt;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_7;
		L_7 = raw_sqlite3_prepare_v3_m5F2871468C2CC403C7DF6EFFD75BACA36C6FE92F(L_3, L_4, L_5, L_6, (&V_2), NULL);
		String_t** L_8 = ___4_tail;
		int32_t L_9;
		L_9 = ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_inline((&V_2), ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		if ((((int32_t)L_9) > ((int32_t)0)))
		{
			G_B2_0 = L_8;
			G_B2_1 = L_7;
			goto IL_002d;
		}
		G_B1_0 = L_8;
		G_B1_1 = L_7;
	}
	{
		G_B3_0 = _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709;
		G_B3_1 = G_B1_0;
		G_B3_2 = G_B1_1;
		goto IL_0043;
	}

IL_002d:
	{
		int32_t L_10;
		L_10 = ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_inline((&V_2), ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_11;
		L_11 = ReadOnlySpan_1_Slice_mEB3D3A427170FC5A0AB734619D4792C299697C89_inline((&V_2), 0, ((int32_t)il2cpp_codegen_subtract(L_10, 1)), ReadOnlySpan_1_Slice_mEB3D3A427170FC5A0AB734619D4792C299697C89_RuntimeMethod_var);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		String_t* L_12;
		L_12 = raw_utf8_span_to_string_m1D3AC8DF369A2FFC9AC7B1A7E165F6B4C7234DE2(L_11, NULL);
		G_B3_0 = L_12;
		G_B3_1 = G_B2_0;
		G_B3_2 = G_B2_1;
	}

IL_0043:
	{
		*((String_t**)G_B3_1) = (String_t*)G_B3_0;
		Il2CppCodeGenWriteBarrier((void**)(String_t**)G_B3_1, (void*)(String_t*)G_B3_0);
		return G_B3_2;
	}
}
// Method Definition Index: 117329
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_exec_m8FAE0AD74C31BE22EFD60DCC0AB4015D4F2F4AE7 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_sql, strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* ___2_callback, RuntimeObject* ___3_user_data, String_t** ___4_errMsg, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass332_0_U3Csqlite3_execU3Eb__0_m09F33029DA46B08016391B82A5E88D8D6CF03F17_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass332_0_t1173985050ED9406B43934B3B874C78753CF564A_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass332_0_t1173985050ED9406B43934B3B874C78753CF564A* V_0 = NULL;
	delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* V_1 = NULL;
	intptr_t V_2;
	memset((&V_2), 0, sizeof(V_2));
	int32_t G_B5_0 = 0;
	int32_t G_B4_0 = 0;
	{
		U3CU3Ec__DisplayClass332_0_t1173985050ED9406B43934B3B874C78753CF564A* L_0 = (U3CU3Ec__DisplayClass332_0_t1173985050ED9406B43934B3B874C78753CF564A*)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass332_0_t1173985050ED9406B43934B3B874C78753CF564A_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass332_0__ctor_m384555F116520FA2FCFE6E072CB793F3802B1934(L_0, NULL);
		V_0 = L_0;
		U3CU3Ec__DisplayClass332_0_t1173985050ED9406B43934B3B874C78753CF564A* L_1 = V_0;
		strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* L_2 = ___2_callback;
		NullCheck(L_1);
		L_1->___callback = L_2;
		Il2CppCodeGenWriteBarrier((void**)(&L_1->___callback), (void*)L_2);
		U3CU3Ec__DisplayClass332_0_t1173985050ED9406B43934B3B874C78753CF564A* L_3 = V_0;
		NullCheck(L_3);
		strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* L_4 = L_3->___callback;
		if (!L_4)
		{
			goto IL_0024;
		}
	}
	{
		U3CU3Ec__DisplayClass332_0_t1173985050ED9406B43934B3B874C78753CF564A* L_5 = V_0;
		delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* L_6 = (delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3*)il2cpp_codegen_object_new(delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3_il2cpp_TypeInfo_var);
		delegate_exec__ctor_m64682E5DA3B842E47174CB722B43977DFCF5AE5B(L_6, L_5, (intptr_t)((void*)U3CU3Ec__DisplayClass332_0_U3Csqlite3_execU3Eb__0_m09F33029DA46B08016391B82A5E88D8D6CF03F17_RuntimeMethod_var), NULL);
		V_1 = L_6;
		goto IL_0026;
	}

IL_0024:
	{
		V_1 = (delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3*)NULL;
	}

IL_0026:
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_7;
		L_7 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_8 = ___0_db;
		String_t* L_9 = ___1_sql;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_10;
		L_10 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_9, NULL);
		delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* L_11 = V_1;
		RuntimeObject* L_12 = ___3_user_data;
		NullCheck(L_7);
		int32_t L_13;
		L_13 = InterfaceFuncInvoker5< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3*, RuntimeObject*, intptr_t* >::Invoke(118, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_7, L_8, L_10, L_11, L_12, (&V_2));
		intptr_t L_14 = V_2;
		bool L_15;
		L_15 = IntPtr_op_Equality_m7D9CDCDE9DC2A0C2C614633F4921E90187FAB271_inline(L_14, 0, NULL);
		if (!L_15)
		{
			G_B5_0 = L_13;
			goto IL_004d;
		}
		G_B4_0 = L_13;
	}
	{
		String_t** L_16 = ___4_errMsg;
		*((String_t**)L_16) = (String_t*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(String_t**)L_16, (void*)(String_t*)NULL);
		return G_B4_0;
	}

IL_004d:
	{
		String_t** L_17 = ___4_errMsg;
		intptr_t L_18 = V_2;
		String_t* L_19;
		L_19 = util_from_utf8_z_mA4AD3FF9FAB5CA653DD808B6B17A04FDF7425743(L_18, NULL);
		*((String_t**)L_17) = (String_t*)L_19;
		Il2CppCodeGenWriteBarrier((void**)(String_t**)L_17, (void*)(String_t*)L_19);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_20;
		L_20 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		intptr_t L_21 = V_2;
		NullCheck(L_20);
		InterfaceActionInvoker1< intptr_t >::Invoke(131, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_20, L_21);
		return G_B5_0;
	}
}
// Method Definition Index: 117330
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_exec_m70254D20CCB8386545CB8FFBB14809C5986CD653 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_sql, String_t** ___2_errMsg, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	int32_t G_B2_0 = 0;
	int32_t G_B1_0 = 0;
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		String_t* L_2 = ___1_sql;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_2, NULL);
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker5< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3*, RuntimeObject*, intptr_t* >::Invoke(118, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_3, (delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3*)NULL, NULL, (&V_0));
		intptr_t L_5 = V_0;
		bool L_6;
		L_6 = IntPtr_op_Equality_m7D9CDCDE9DC2A0C2C614633F4921E90187FAB271_inline(L_5, 0, NULL);
		if (!L_6)
		{
			G_B2_0 = L_4;
			goto IL_0026;
		}
		G_B1_0 = L_4;
	}
	{
		String_t** L_7 = ___2_errMsg;
		*((String_t**)L_7) = (String_t*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(String_t**)L_7, (void*)(String_t*)NULL);
		return G_B1_0;
	}

IL_0026:
	{
		String_t** L_8 = ___2_errMsg;
		intptr_t L_9 = V_0;
		String_t* L_10;
		L_10 = util_from_utf8_z_mA4AD3FF9FAB5CA653DD808B6B17A04FDF7425743(L_9, NULL);
		*((String_t**)L_8) = (String_t*)L_10;
		Il2CppCodeGenWriteBarrier((void**)(String_t**)L_8, (void*)(String_t*)L_10);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_11;
		L_11 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		intptr_t L_12 = V_0;
		NullCheck(L_11);
		InterfaceActionInvoker1< intptr_t >::Invoke(131, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_11, L_12);
		return G_B2_0;
	}
}
// Method Definition Index: 117331
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_exec_mE5A0472E933929D04DF0421FD0E9E3FBC034B621 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_sql, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	int32_t G_B2_0 = 0;
	int32_t G_B1_0 = 0;
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		String_t* L_2 = ___1_sql;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_2, NULL);
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker5< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3*, RuntimeObject*, intptr_t* >::Invoke(118, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_3, (delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3*)NULL, NULL, (&V_0));
		intptr_t L_5 = V_0;
		bool L_6;
		L_6 = IntPtr_op_Equality_m7D9CDCDE9DC2A0C2C614633F4921E90187FAB271_inline(L_5, 0, NULL);
		if (L_6)
		{
			G_B2_0 = L_4;
			goto IL_002d;
		}
		G_B1_0 = L_4;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_7;
		L_7 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		intptr_t L_8 = V_0;
		NullCheck(L_7);
		InterfaceActionInvoker1< intptr_t >::Invoke(131, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_7, L_8);
		G_B2_0 = G_B1_0;
	}

IL_002d:
	{
		return G_B2_0;
	}
}
// Method Definition Index: 117332
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_step_m6E92CF3EE7CC3B78DBA74BB01E648F770082EAF2 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* >::Invoke(33, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117333
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_finalize_m0670A0E6707B3B551B652FADB6A2455792F86372 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, const RuntimeMethod* method) 
{
	{
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_0 = ___0_stmt;
		NullCheck(L_0);
		int32_t L_1;
		L_1 = sqlite3_stmt_manual_close_m3A3C2F6C6782CB371A60EA7B5372F9CCA721577A(L_0, NULL);
		return L_1;
	}
}
// Method Definition Index: 117334
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_internal_sqlite3_finalize_m4A8747B9480EA6234BC79A647F029851A3A99A41 (intptr_t ___0_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		intptr_t L_1 = ___0_stmt;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, intptr_t >::Invoke(34, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117335
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_reset_m8C0EF2800CCD288A208A373B0BA73A989F837438 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* >::Invoke(35, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117336
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_clear_bindings_m4F812B7AF4AD642503C1C6CF34901ECC4270BF0B (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* >::Invoke(36, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117337
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_stmt_status_m146D194A3727EC9D5CD06F1FB919C93C4F96CC1F (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_op, int32_t ___2_resetFlg, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_op;
		int32_t L_3 = ___2_resetFlg;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t, int32_t >::Invoke(37, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117338
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_complete_m40E74786ADC4CF27376C9AADFCE23F33E939E869 (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___0_sql, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_1 = ___0_sql;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(119, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117339
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_complete_m384571D8B38001A1701BF10D535EE1FCFD33D7F5 (String_t* ___0_sql, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		String_t* L_0 = ___0_sql;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_1;
		L_1 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_0, NULL);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_2;
		L_2 = raw_sqlite3_complete_m40E74786ADC4CF27376C9AADFCE23F33E939E869(L_1, NULL);
		return L_2;
	}
}
// Method Definition Index: 117340
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_compileoption_used_m9905722912C4F69D2735DA5349726FB1056BF621 (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___0_s, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_1 = ___0_s;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(120, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117341
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_compileoption_used_mB26F27E89A3002873DD433C4647EC18EC7377834 (String_t* ___0_s, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		String_t* L_0 = ___0_s;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_1;
		L_1 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_0, NULL);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_2;
		L_2 = raw_sqlite3_compileoption_used_m9905722912C4F69D2735DA5349726FB1056BF621(L_1, NULL);
		return L_2;
	}
}
// Method Definition Index: 117342
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_compileoption_get_mC25391F6C9B888BB02A8CA512C0B4E9439A682C9 (int32_t ___0_n, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		int32_t L_1 = ___0_n;
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		L_2 = InterfaceFuncInvoker1< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int32_t >::Invoke(121, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117343
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_table_column_metadata_mB544F7219F7E00FD72E574C5B1A7011165D4AA93 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_dbName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_tblName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_colName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* ___4_dataType, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* ___5_collSeq, int32_t* ___6_notNull, int32_t* ___7_primaryKey, int32_t* ___8_autoInc, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___1_dbName;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3 = ___2_tblName;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_4 = ___3_colName;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* L_5 = ___4_dataType;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* L_6 = ___5_collSeq;
		int32_t* L_7 = ___6_notNull;
		int32_t* L_8 = ___7_primaryKey;
		int32_t* L_9 = ___8_autoInc;
		NullCheck(L_0);
		int32_t L_10;
		L_10 = InterfaceFuncInvoker9< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25*, int32_t*, int32_t*, int32_t* >::Invoke(125, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3, L_4, L_5, L_6, L_7, L_8, L_9);
		return L_10;
	}
}
// Method Definition Index: 117344
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_table_column_metadata_m6C42E789DC6881593EAE72D144A9D1EB41503B4D (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_dbName, String_t* ___2_tblName, String_t* ___3_colName, String_t** ___4_dataType, String_t** ___5_collSeq, int32_t* ___6_notNull, int32_t* ___7_primaryKey, int32_t* ___8_autoInc, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 V_0;
	memset((&V_0), 0, sizeof(V_0));
	utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_0 = ___0_db;
		String_t* L_1 = ___1_dbName;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		L_2 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_1, NULL);
		String_t* L_3 = ___2_tblName;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_4;
		L_4 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_3, NULL);
		String_t* L_5 = ___3_colName;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_6;
		L_6 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_5, NULL);
		int32_t* L_7 = ___6_notNull;
		int32_t* L_8 = ___7_primaryKey;
		int32_t* L_9 = ___8_autoInc;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_10;
		L_10 = raw_sqlite3_table_column_metadata_mB544F7219F7E00FD72E574C5B1A7011165D4AA93(L_0, L_2, L_4, L_6, (&V_0), (&V_1), L_7, L_8, L_9, NULL);
		String_t** L_11 = ___4_dataType;
		String_t* L_12;
		L_12 = utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A((&V_0), NULL);
		*((String_t**)L_11) = (String_t*)L_12;
		Il2CppCodeGenWriteBarrier((void**)(String_t**)L_11, (void*)(String_t*)L_12);
		String_t** L_13 = ___5_collSeq;
		String_t* L_14;
		L_14 = utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A((&V_1), NULL);
		*((String_t**)L_13) = (String_t*)L_14;
		Il2CppCodeGenWriteBarrier((void**)(String_t**)L_13, (void*)(String_t*)L_14);
		return L_10;
	}
}
// Method Definition Index: 117345
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_sql_m6D5A26ADCC81086916D9D3A7DDD4EC18694F6A2F (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		L_2 = InterfaceFuncInvoker1< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* >::Invoke(38, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117346
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* raw_sqlite3_db_handle_mD2BF7B55D27D84695DF92CA2F0ACA901A87BD34C (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, const RuntimeMethod* method) 
{
	{
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_0 = ___0_stmt;
		NullCheck(L_0);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1;
		L_1 = sqlite3_stmt_get_db_m31DCFD46B918941ED21CAD5F953201B59EF259CF_inline(L_0, NULL);
		return L_1;
	}
}
// Method Definition Index: 117347
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* raw_sqlite3_next_stmt_m22052500A529B591DE1CAB3ABEE96AE476792B43 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___1_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* G_B2_0 = NULL;
	RuntimeObject* G_B2_1 = NULL;
	sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* G_B1_0 = NULL;
	RuntimeObject* G_B1_1 = NULL;
	intptr_t G_B3_0;
	memset((&G_B3_0), 0, sizeof(G_B3_0));
	sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* G_B3_1 = NULL;
	RuntimeObject* G_B3_2 = NULL;
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_2 = ___1_stmt;
		if (L_2)
		{
			G_B2_0 = L_1;
			G_B2_1 = L_0;
			goto IL_0010;
		}
		G_B1_0 = L_1;
		G_B1_1 = L_0;
	}
	{
		G_B3_0 = 0;
		G_B3_1 = G_B1_0;
		G_B3_2 = G_B1_1;
		goto IL_0016;
	}

IL_0010:
	{
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_3 = ___1_stmt;
		NullCheck(L_3);
		intptr_t L_4;
		L_4 = sqlite3_stmt_get_ptr_mF5030B60EB110512D248A632A61AE2F899580B58_inline(L_3, NULL);
		G_B3_0 = L_4;
		G_B3_1 = G_B2_0;
		G_B3_2 = G_B2_1;
	}

IL_0016:
	{
		NullCheck(G_B3_2);
		intptr_t L_5;
		L_5 = InterfaceFuncInvoker2< intptr_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, intptr_t >::Invoke(40, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, G_B3_2, G_B3_1, G_B3_0);
		V_0 = L_5;
		intptr_t L_6 = V_0;
		bool L_7;
		L_7 = IntPtr_op_Equality_m7D9CDCDE9DC2A0C2C614633F4921E90187FAB271_inline(L_6, 0, NULL);
		if (!L_7)
		{
			goto IL_002b;
		}
	}
	{
		return (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)NULL;
	}

IL_002b:
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_8 = ___0_db;
		intptr_t L_9 = V_0;
		NullCheck(L_8);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_10;
		L_10 = sqlite3_find_stmt_m6DB56FD791338E8E5524252CE51E90D7D6624FE3(L_8, L_9, NULL);
		return L_10;
	}
}
// Method Definition Index: 117348
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_zeroblob_m6EC009EC660362DCF35E31ACE228A385750DC26D (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, int32_t ___2_size, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		int32_t L_3 = ___2_size;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t, int32_t >::Invoke(41, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117349
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_bind_parameter_name_m26150C415633B54421F56208DF6CC5EB24D4F45A (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = InterfaceFuncInvoker2< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t >::Invoke(42, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117350
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* raw_sqlite3_user_data_m64779B2A1A79BEC548BFDD73524A4E9C9C9C515D (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, const RuntimeMethod* method) 
{
	{
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_0 = ___0_context;
		NullCheck(L_0);
		RuntimeObject* L_1;
		L_1 = sqlite3_context_get_user_data_m2B56AA945EBDDC7B032007BA993E2B0D6311E609_inline(L_0, NULL);
		return L_1;
	}
}
// Method Definition Index: 117351
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_null_m83B5287C368792C1F5CE7F76D940D2CDB25AE48D (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_1 = ___0_context;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline(L_1, NULL);
		NullCheck(L_0);
		InterfaceActionInvoker1< intptr_t >::Invoke(101, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2);
		return;
	}
}
// Method Definition Index: 117352
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_blob_m28FB048DC4BFD915AC7C1E088758C0A2FAD3C57F (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_1 = ___0_context;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline(L_1, NULL);
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_3 = ___1_val;
		NullCheck(L_0);
		InterfaceActionInvoker2< intptr_t, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D >::Invoke(95, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117353
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_error_mECD8AFA7C55E4C92DB3FDB25580AB44FC0C59657 (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_1 = ___0_context;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline(L_1, NULL);
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_3 = ___1_val;
		NullCheck(L_0);
		InterfaceActionInvoker2< intptr_t, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D >::Invoke(97, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117354
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_error_m07EEA2401F42F29ACD02EB8950E259A4EB769D21 (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_1 = ___0_context;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline(L_1, NULL);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3 = ___1_val;
		NullCheck(L_0);
		InterfaceActionInvoker2< intptr_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(98, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117355
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_error_mB0C4D6C743E1A553D4F2DCB2E3FD50CCDB2EA9EA (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, String_t* ___1_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_0 = ___0_context;
		String_t* L_1 = ___1_val;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		L_2 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_1, NULL);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		raw_sqlite3_result_error_m07EEA2401F42F29ACD02EB8950E259A4EB769D21(L_0, L_2, NULL);
		return;
	}
}
// Method Definition Index: 117356
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_text_mAF9B57849F55BCAAFBE579E6FE1A3400AE5579FC (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_1 = ___0_context;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline(L_1, NULL);
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_3 = ___1_val;
		NullCheck(L_0);
		InterfaceActionInvoker2< intptr_t, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D >::Invoke(102, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117357
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_text_mB5E6D83F397C4F537EE93458DDB98B15AA8A830B (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_1 = ___0_context;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline(L_1, NULL);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3 = ___1_val;
		NullCheck(L_0);
		InterfaceActionInvoker2< intptr_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(103, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117358
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_text_mF6698071C00A1A902307988E073F1B2551C583BA (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, String_t* ___1_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_0 = ___0_context;
		String_t* L_1 = ___1_val;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		L_2 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_1, NULL);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		raw_sqlite3_result_text_mB5E6D83F397C4F537EE93458DDB98B15AA8A830B(L_0, L_2, NULL);
		return;
	}
}
// Method Definition Index: 117359
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_double_m26F4A3CF86C695B63A460D18F09FBF36222C05CA (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, double ___1_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_1 = ___0_context;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline(L_1, NULL);
		double L_3 = ___1_val;
		NullCheck(L_0);
		InterfaceActionInvoker2< intptr_t, double >::Invoke(96, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117360
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_int_m3FC9FB145E5EF8CD98B980374A5EF80C132B2919 (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, int32_t ___1_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_1 = ___0_context;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline(L_1, NULL);
		int32_t L_3 = ___1_val;
		NullCheck(L_0);
		InterfaceActionInvoker2< intptr_t, int32_t >::Invoke(99, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117361
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_int64_mFC0125DED1576E69DA39A76066EFDD372A907250 (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, int64_t ___1_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_1 = ___0_context;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline(L_1, NULL);
		int64_t L_3 = ___1_val;
		NullCheck(L_0);
		InterfaceActionInvoker2< intptr_t, int64_t >::Invoke(100, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117362
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_zeroblob_mD0415B4365E2341F0BC02E1A1D31F2BEB1EBEFFC (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, int32_t ___1_n, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_1 = ___0_context;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline(L_1, NULL);
		int32_t L_3 = ___1_n;
		NullCheck(L_0);
		InterfaceActionInvoker2< intptr_t, int32_t >::Invoke(104, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117363
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_error_toobig_m4AF19AB169ACB7F5C0F304E19C69D2C86EC886E1 (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_1 = ___0_context;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline(L_1, NULL);
		NullCheck(L_0);
		InterfaceActionInvoker1< intptr_t >::Invoke(105, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2);
		return;
	}
}
// Method Definition Index: 117364
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_error_nomem_m8C0093E19AC9A0095276CD6325F89A090D4C837C (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_1 = ___0_context;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline(L_1, NULL);
		NullCheck(L_0);
		InterfaceActionInvoker1< intptr_t >::Invoke(106, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2);
		return;
	}
}
// Method Definition Index: 117365
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_result_error_code_m30CA7F9B6A00342BA5337401BE7A67A7DA323B90 (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_context, int32_t ___1_code, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_1 = ___0_context;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline(L_1, NULL);
		int32_t L_3 = ___1_code;
		NullCheck(L_0);
		InterfaceActionInvoker2< intptr_t, int32_t >::Invoke(107, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2, L_3);
		return;
	}
}
// Method Definition Index: 117366
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D raw_sqlite3_value_blob_m5561477B51D647DBF6349E77ED2D75B679CD9B05 (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* ___0_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* L_1 = ___0_val;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_value_get_ptr_mAF588DD3703B49533D61586741960A38A4515AD9_inline(L_1, NULL);
		NullCheck(L_0);
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_3;
		L_3 = InterfaceFuncInvoker1< ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, intptr_t >::Invoke(108, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2);
		return L_3;
	}
}
// Method Definition Index: 117367
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_value_bytes_mC1A9D8917AD56C4640F953F5FAEA539C80C62D42 (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* ___0_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* L_1 = ___0_val;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_value_get_ptr_mAF588DD3703B49533D61586741960A38A4515AD9_inline(L_1, NULL);
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker1< int32_t, intptr_t >::Invoke(109, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2);
		return L_3;
	}
}
// Method Definition Index: 117368
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR double raw_sqlite3_value_double_m7BADB83572F0556831141C439E2CD1DC0DE20F14 (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* ___0_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* L_1 = ___0_val;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_value_get_ptr_mAF588DD3703B49533D61586741960A38A4515AD9_inline(L_1, NULL);
		NullCheck(L_0);
		double L_3;
		L_3 = InterfaceFuncInvoker1< double, intptr_t >::Invoke(110, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2);
		return L_3;
	}
}
// Method Definition Index: 117369
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_value_int_m4183478D4B1B34260BE6FA3D2167ECAD9C95505F (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* ___0_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* L_1 = ___0_val;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_value_get_ptr_mAF588DD3703B49533D61586741960A38A4515AD9_inline(L_1, NULL);
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker1< int32_t, intptr_t >::Invoke(111, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2);
		return L_3;
	}
}
// Method Definition Index: 117370
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t raw_sqlite3_value_int64_m69DA9596A93DCA093C8785D908FCC01D831D1BFF (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* ___0_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* L_1 = ___0_val;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_value_get_ptr_mAF588DD3703B49533D61586741960A38A4515AD9_inline(L_1, NULL);
		NullCheck(L_0);
		int64_t L_3;
		L_3 = InterfaceFuncInvoker1< int64_t, intptr_t >::Invoke(112, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2);
		return L_3;
	}
}
// Method Definition Index: 117371
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_value_type_m6C151D3A6A258C287EC5F5A99C076E7433B502ED (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* ___0_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* L_1 = ___0_val;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_value_get_ptr_mAF588DD3703B49533D61586741960A38A4515AD9_inline(L_1, NULL);
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker1< int32_t, intptr_t >::Invoke(113, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2);
		return L_3;
	}
}
// Method Definition Index: 117372
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_value_text_m2A5EE0FDF1198D6B14BDE854C8BF0F886E01E8E8 (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* ___0_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* L_1 = ___0_val;
		NullCheck(L_1);
		intptr_t L_2;
		L_2 = sqlite3_value_get_ptr_mAF588DD3703B49533D61586741960A38A4515AD9_inline(L_1, NULL);
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = InterfaceFuncInvoker1< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, intptr_t >::Invoke(114, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_2);
		return L_3;
	}
}
// Method Definition Index: 117373
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_blob_m17633379D6404CD4C3B43057ECC2E05079B217F9 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_blob, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_3 = ___2_blob;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D >::Invoke(43, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117374
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_double_mBF982A0FE68453C1673F48E640F6E57D13F11E9D (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, double ___2_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		double L_3 = ___2_val;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t, double >::Invoke(44, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117375
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_int_m31DB436436AEB4470AC793C1184F95F0CF9ACFB1 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, int32_t ___2_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		int32_t L_3 = ___2_val;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t, int32_t >::Invoke(45, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117376
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_int64_m88972EE150F56A72539E512D8B54E85D7CC74F9D (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, int64_t ___2_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		int64_t L_3 = ___2_val;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t, int64_t >::Invoke(46, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117377
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_null_m4CA69F96F36C6BE90D82BDC5ED9DEB04FEF021FD (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t >::Invoke(47, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117378
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_text_m25D027C3A22B8942CD029A94A14E3B72091F5B9C (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_3 = ___2_val;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D >::Invoke(48, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117379
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_text16_mD6248B3F0776BB8278506BDB03331DDBDDF73C2B (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, ReadOnlySpan_1_t59614EA6E51A945A32B02AB17FBCBDF9A5C419C1 ___2_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		ReadOnlySpan_1_t59614EA6E51A945A32B02AB17FBCBDF9A5C419C1 L_3 = ___2_val;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t, ReadOnlySpan_1_t59614EA6E51A945A32B02AB17FBCBDF9A5C419C1 >::Invoke(49, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117380
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_text_m0DC3ED8746CA56D5D9531F8C2438240B4A664DD2 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3 = ___2_val;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(50, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117381
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_text_m89FBF3ED41568FC8DD9F267F41DE99B589A114FA (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, String_t* ___2_val, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Span_1_GetPinnableReference_m55DA180AC02A047DAC0626C7B8CBC2E87626DD0C_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Span_1__ctor_mE18EBB601FBFA01BA29FE353364700952A9091FE_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Span_1_op_Implicit_mD249188242C0C9D3192A31E9F7FA74C683F05B84_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305 V_1;
	memset((&V_1), 0, sizeof(V_1));
	int32_t V_2 = 0;
	Il2CppChar* V_3 = NULL;
	String_t* V_4 = NULL;
	uint8_t* V_5 = NULL;
	uint8_t* V_6 = NULL;
	{
		String_t* L_0 = ___2_val;
		if (!L_0)
		{
			goto IL_007f;
		}
	}
	{
		String_t* L_1 = ___2_val;
		NullCheck(L_1);
		int32_t L_2;
		L_2 = String_get_Length_m42625D67623FA5CC7A44D47425CE86FB946542D2_inline(L_1, NULL);
		if ((((int32_t)L_2) > ((int32_t)((int32_t)512))))
		{
			goto IL_007f;
		}
	}
	{
		Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* L_3;
		L_3 = Encoding_get_UTF8_m9FA98A53CE96FD6D02982625C5246DD36C1235C9(NULL);
		String_t* L_4 = ___2_val;
		NullCheck(L_3);
		int32_t L_5;
		L_5 = VirtualFuncInvoker1< int32_t, String_t* >::Invoke(11, L_3, L_4);
		V_0 = L_5;
		int32_t L_6 = V_0;
		if ((((int32_t)L_6) > ((int32_t)((int32_t)512))))
		{
			goto IL_007f;
		}
	}
	{
		int32_t L_7 = V_0;
		if ((((int32_t)L_7) <= ((int32_t)0)))
		{
			goto IL_007f;
		}
	}
	{
		int32_t L_8 = V_0;
		V_2 = L_8;
		int32_t L_9 = V_2;
		uintptr_t L_10 = ((uintptr_t)L_9);
		int8_t* L_11 = (int8_t*) (L_10 ? alloca(L_10) : NULL);
		memset(L_11, 0, L_10);
		int32_t L_12 = V_2;
		Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305 L_13;
		memset((&L_13), 0, sizeof(L_13));
		Span_1__ctor_mE18EBB601FBFA01BA29FE353364700952A9091FE_inline((&L_13), (void*)(L_11), L_12, Span_1__ctor_mE18EBB601FBFA01BA29FE353364700952A9091FE_RuntimeMethod_var);
		V_1 = L_13;
		String_t* L_14 = ___2_val;
		V_4 = L_14;
		String_t* L_15 = V_4;
		V_3 = (Il2CppChar*)((uintptr_t)L_15);
		Il2CppChar* L_16 = V_3;
		if (!L_16)
		{
			goto IL_0047;
		}
	}
	{
		Il2CppChar* L_17 = V_3;
		int32_t L_18;
		L_18 = RuntimeHelpers_get_OffsetToStringData_m90A5D27EF88BE9432BF7093B7D7E7A0ACB0A8FBD(NULL);
		V_3 = ((Il2CppChar*)il2cpp_codegen_add((intptr_t)L_17, L_18));
	}

IL_0047:
	{
		uint8_t* L_19;
		L_19 = Span_1_GetPinnableReference_m55DA180AC02A047DAC0626C7B8CBC2E87626DD0C((&V_1), Span_1_GetPinnableReference_m55DA180AC02A047DAC0626C7B8CBC2E87626DD0C_RuntimeMethod_var);
		V_6 = L_19;
		uint8_t* L_20 = V_6;
		V_5 = (uint8_t*)((uintptr_t)L_20);
		Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* L_21;
		L_21 = Encoding_get_UTF8_m9FA98A53CE96FD6D02982625C5246DD36C1235C9(NULL);
		Il2CppChar* L_22 = V_3;
		String_t* L_23 = ___2_val;
		NullCheck(L_23);
		int32_t L_24;
		L_24 = String_get_Length_m42625D67623FA5CC7A44D47425CE86FB946542D2_inline(L_23, NULL);
		uint8_t* L_25 = V_5;
		int32_t L_26 = V_0;
		NullCheck(L_21);
		int32_t L_27;
		L_27 = VirtualFuncInvoker4< int32_t, Il2CppChar*, int32_t, uint8_t*, int32_t >::Invoke(21, L_21, L_22, L_24, L_25, L_26);
		V_6 = (uint8_t*)((uintptr_t)0);
		V_4 = (String_t*)NULL;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_28 = ___0_stmt;
		int32_t L_29 = ___1_index;
		Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305 L_30 = V_1;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_31;
		L_31 = Span_1_op_Implicit_mD249188242C0C9D3192A31E9F7FA74C683F05B84(L_30, Span_1_op_Implicit_mD249188242C0C9D3192A31E9F7FA74C683F05B84_RuntimeMethod_var);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_32;
		L_32 = raw_sqlite3_bind_text_m25D027C3A22B8942CD029A94A14E3B72091F5B9C(L_28, L_29, L_31, NULL);
		return L_32;
	}

IL_007f:
	{
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_33 = ___0_stmt;
		int32_t L_34 = ___1_index;
		String_t* L_35 = ___2_val;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_36;
		L_36 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_35, NULL);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_37;
		L_37 = raw_sqlite3_bind_text_m0DC3ED8746CA56D5D9531F8C2438240B4A664DD2(L_33, L_34, L_36, NULL);
		return L_37;
	}
}
// Method Definition Index: 117382
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_parameter_count_m2138DB1239B266A04C8E744AA8E2287AF53F5A2F (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* >::Invoke(51, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117383
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_parameter_index_m4477224407A1C70D93BB68EF993CAADBC2CD4F28 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_strName, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___1_strName;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(52, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117384
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_bind_parameter_index_m6B3082259F81F8C9E39764385B6FF7AB2D7EA6B6 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, String_t* ___1_strName, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_0 = ___0_stmt;
		String_t* L_1 = ___1_strName;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		L_2 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_1, NULL);
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_3;
		L_3 = raw_sqlite3_bind_parameter_index_m4477224407A1C70D93BB68EF993CAADBC2CD4F28(L_0, L_2, NULL);
		return L_3;
	}
}
// Method Definition Index: 117385
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_stmt_isexplain_m6153F0F7F1229E1D5279FCC79081985901764AB9 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0 = ((raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_StaticFields*)il2cpp_codegen_static_fields_for(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var))->____imp;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* >::Invoke(115, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117386
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_stmt_busy_m5B52667D78ECFF1C3082767087311DF3EC456820 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* >::Invoke(116, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117387
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_stmt_readonly_mB3762C5D5F880AE752A36EE68E40478658FD9851 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* >::Invoke(117, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117388
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_column_database_name_m489CEC0010AFA408044F951958C0EACC5E7CCA28 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = InterfaceFuncInvoker2< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t >::Invoke(53, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117389
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_column_name_m0A6D0E7DDAB130304E83CE2D84A6A7384EFDC8FF (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = InterfaceFuncInvoker2< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t >::Invoke(54, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117390
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_column_origin_name_m49EF179CF9B2B4F30AFE4887F8712385D02F5FF5 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = InterfaceFuncInvoker2< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t >::Invoke(55, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117391
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_column_table_name_m47ED42C98BD19563F9D88323300FA748623FC8E1 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = InterfaceFuncInvoker2< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t >::Invoke(56, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117392
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_column_text_m5A212B9A6D05C5762B8ED267692971CE8689748A (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = InterfaceFuncInvoker2< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t >::Invoke(57, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117393
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_column_count_m1B264B86A3B47ECC7B258086F8E88DE8BB625B68 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* >::Invoke(59, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117394
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_data_count_m965DF8D640E47C83F3BD3EC8E584CA304C88A0BE (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* >::Invoke(58, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117395
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR double raw_sqlite3_column_double_m7BE24F414F73C682D1A3930ADE8FABB3D44D0800 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		NullCheck(L_0);
		double L_3;
		L_3 = InterfaceFuncInvoker2< double, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t >::Invoke(60, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117396
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_column_int_mC681A2F7E7D23E4D8E593C7034563958ABCBB670 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t >::Invoke(61, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117397
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t raw_sqlite3_column_int64_mDA51434415E121925115A3D15D1FB7902BB49EEA (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		NullCheck(L_0);
		int64_t L_3;
		L_3 = InterfaceFuncInvoker2< int64_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t >::Invoke(62, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117398
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D raw_sqlite3_column_blob_m97CCA52D3A8E6E575CC57089174CCBC73D750C23 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		NullCheck(L_0);
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_3;
		L_3 = InterfaceFuncInvoker2< ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t >::Invoke(63, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117399
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_column_bytes_m715D7DB64E328C1F3B8C4BD45367EB3720A8901C (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t >::Invoke(64, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117400
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_column_type_m0FA702F0B761ED282CF8C48A982AF8F3BC1EDF5B (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t >::Invoke(65, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117401
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 raw_sqlite3_column_decltype_mDC9885BF3415F0DEEB6ACB8A8323BB79712A52E7 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, int32_t ___1_index, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = ___0_stmt;
		int32_t L_2 = ___1_index;
		NullCheck(L_0);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = InterfaceFuncInvoker2< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*, int32_t >::Invoke(66, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117402
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* raw_sqlite3_backup_init_m78558F43AA78828A58CA34ABF5489B6F4029B7A3 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_destDb, String_t* ___1_destName, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___2_sourceDb, String_t* ___3_sourceName, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_destDb;
		String_t* L_2 = ___1_destName;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_2, NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_4 = ___2_sourceDb;
		String_t* L_5 = ___3_sourceName;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_6;
		L_6 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_5, NULL);
		NullCheck(L_0);
		sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* L_7;
		L_7 = InterfaceFuncInvoker4< sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30*, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(72, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_3, L_4, L_6);
		return L_7;
	}
}
// Method Definition Index: 117403
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_backup_step_m134C0A19246992A29FEE4FAC6F98F5AF850B963B (sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* ___0_backup, int32_t ___1_nPage, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* L_1 = ___0_backup;
		int32_t L_2 = ___1_nPage;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30*, int32_t >::Invoke(73, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117404
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_backup_remaining_m76C9C87C7335A4E998F363E087DCB8D04EC08B72 (sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* ___0_backup, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* L_1 = ___0_backup;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* >::Invoke(74, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117405
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_backup_pagecount_m52F5D8872978FFB51C6727F8BF530F75CA2EC988 (sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* ___0_backup, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* L_1 = ___0_backup;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* >::Invoke(75, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117406
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_backup_finish_m9BB17E5B7D4FCE8C9874A590CB65F2465CA1C764 (sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* ___0_backup, const RuntimeMethod* method) 
{
	{
		sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* L_0 = ___0_backup;
		NullCheck(L_0);
		int32_t L_1;
		L_1 = sqlite3_backup_manual_close_m16BBF17DEA8FFABBE26C4A1668815FA624897244(L_0, NULL);
		return L_1;
	}
}
// Method Definition Index: 117407
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_internal_sqlite3_backup_finish_m84D9D7C6128794ADADBF3757E4587590A8749DC1 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		intptr_t L_1 = ___0_p;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, intptr_t >::Invoke(76, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117408
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_snapshot_get_m8D1C73985B9E49D8841D7B05651521321A96EBED (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_schema, sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475** ___2_snap, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		String_t* L_2 = ___1_schema;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_2, NULL);
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, intptr_t* >::Invoke(67, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_3, (&V_0));
		sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475** L_5 = ___2_snap;
		intptr_t L_6 = V_0;
		sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* L_7;
		L_7 = sqlite3_snapshot_From_mFC6D90D41F3A68F30767F7A792A370CFF7D39E83(L_6, NULL);
		*((sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475**)L_5) = (sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475*)L_7;
		Il2CppCodeGenWriteBarrier((void**)(sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475**)L_5, (void*)(sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475*)L_7);
		return L_4;
	}
}
// Method Definition Index: 117409
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_snapshot_cmp_mC6B925E693BC49DBEA3FF8095CA666C906928F30 (sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* ___0_p1, sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* ___1_p2, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* L_1 = ___0_p1;
		sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* L_2 = ___1_p2;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475*, sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* >::Invoke(68, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117410
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_snapshot_open_m6271EF62B7C9BBA0DAF6E67ADFF85982D9B09245 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_schema, sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* ___2_snap, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		String_t* L_2 = ___1_schema;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_2, NULL);
		sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* L_4 = ___2_snap;
		NullCheck(L_0);
		int32_t L_5;
		L_5 = InterfaceFuncInvoker3< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* >::Invoke(69, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_3, L_4);
		return L_5;
	}
}
// Method Definition Index: 117411
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_snapshot_recover_mF5BE143CCD4DC1B76265F0C0F6943EBEAA557AED (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_name, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		String_t* L_2 = ___1_name;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_2, NULL);
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker2< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(70, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_3);
		return L_4;
	}
}
// Method Definition Index: 117412
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_snapshot_free_mEAA70C1A8CB409D76FB1D73C1DC4A59D9C8A4F0B (sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* ___0_snap, const RuntimeMethod* method) 
{
	{
		sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* L_0 = ___0_snap;
		NullCheck(L_0);
		sqlite3_snapshot_manual_close_m49FE953A1E7ACF28AA2C4A2C7EF8DEC9A953D9C8(L_0, NULL);
		return;
	}
}
// Method Definition Index: 117413
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_internal_sqlite3_snapshot_free_mB9410BEFC8368B7ECE442E6CB7D8725A51649FD3 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		intptr_t L_1 = ___0_p;
		NullCheck(L_0);
		InterfaceActionInvoker1< intptr_t >::Invoke(71, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return;
	}
}
// Method Definition Index: 117414
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_blob_open_m16A7C4AD7CCA801A481F634DADC1E866505532C5 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_db_utf8, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_table_utf8, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_col_utf8, int64_t ___4_rowid, int32_t ___5_flags, sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057** ___6_blob, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___1_db_utf8;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3 = ___2_table_utf8;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_4 = ___3_col_utf8;
		int64_t L_5 = ___4_rowid;
		int32_t L_6 = ___5_flags;
		sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057** L_7 = ___6_blob;
		NullCheck(L_0);
		int32_t L_8;
		L_8 = InterfaceFuncInvoker7< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t, int32_t, sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057** >::Invoke(77, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3, L_4, L_5, L_6, L_7);
		return L_8;
	}
}
// Method Definition Index: 117415
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_blob_open_m43A54BA77ED38EFC29D2ACBAE8FA0ABF651ED70E (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_sdb, String_t* ___2_table, String_t* ___3_col, int64_t ___4_rowid, int32_t ___5_flags, sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057** ___6_blob, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_0 = ___0_db;
		String_t* L_1 = ___1_sdb;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		L_2 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_1, NULL);
		String_t* L_3 = ___2_table;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_4;
		L_4 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_3, NULL);
		String_t* L_5 = ___3_col;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_6;
		L_6 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_5, NULL);
		int64_t L_7 = ___4_rowid;
		int32_t L_8 = ___5_flags;
		sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057** L_9 = ___6_blob;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_10;
		L_10 = raw_sqlite3_blob_open_m16A7C4AD7CCA801A481F634DADC1E866505532C5(L_0, L_2, L_4, L_6, L_7, L_8, L_9, NULL);
		return L_10;
	}
}
// Method Definition Index: 117416
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_blob_bytes_m6881167C669D2E2ECDC909D1EA25CF46B41CC87E (sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* ___0_blob, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* L_1 = ___0_blob;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* >::Invoke(78, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117417
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_blob_reopen_m5FFDC4CCDD06DCDB55548D4E7EA81CF9674D2961 (sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* ___0_blob, int64_t ___1_rowid, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* L_1 = ___0_blob;
		int64_t L_2 = ___1_rowid;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057*, int64_t >::Invoke(79, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117418
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_blob_write_mD7E82B87873B4B5E40125335204B77A198E90ECA (sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* ___0_blob, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_b, int32_t ___2_offset, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* L_1 = ___0_blob;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_2 = ___1_b;
		int32_t L_3 = ___2_offset;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, int32_t >::Invoke(80, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117419
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_blob_read_mB8F712A1CEC1002D5DAAD5B8667133E7E4F57A43 (sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* ___0_blob, Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305 ___1_b, int32_t ___2_offset, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* L_1 = ___0_blob;
		Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305 L_2 = ___1_b;
		int32_t L_3 = ___2_offset;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057*, Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305, int32_t >::Invoke(81, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117420
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_blob_close_m42E33C33619E0ACF3C811A8B6CCF76E16ACF3E0D (sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* ___0_blob, const RuntimeMethod* method) 
{
	{
		sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* L_0 = ___0_blob;
		NullCheck(L_0);
		int32_t L_1;
		L_1 = sqlite3_blob_manual_close_m32537CD8CDCC89D1CBAD7832CFA559A05C61BA5F(L_0, NULL);
		return L_1;
	}
}
// Method Definition Index: 117421
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_internal_sqlite3_blob_close_mF4D383511A58F4241E7337A440194216B4505DEC (intptr_t ___0_blob, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		intptr_t L_1 = ___0_blob;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = InterfaceFuncInvoker1< int32_t, intptr_t >::Invoke(82, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117422
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_wal_autocheckpoint_m5F52D2A132904CA4B84A0C1E28FE5E3BB507416D (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, int32_t ___1_n, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		int32_t L_2 = ___1_n;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, int32_t >::Invoke(122, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117423
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_wal_checkpoint_mCC7BF8CE6F2F9E840CC4CA5A228A6638F4C169E0 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_dbName, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		String_t* L_2 = ___1_dbName;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_2, NULL);
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker2< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(123, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_3);
		return L_4;
	}
}
// Method Definition Index: 117424
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_wal_checkpoint_v2_m9E3A0EA31CD50EAB5FC5285B9C3A161FDD90BB23 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_dbName, int32_t ___2_eMode, int32_t* ___3_logSize, int32_t* ___4_framesCheckPointed, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		String_t* L_2 = ___1_dbName;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_2, NULL);
		int32_t L_4 = ___2_eMode;
		int32_t* L_5 = ___3_logSize;
		int32_t* L_6 = ___4_framesCheckPointed;
		NullCheck(L_0);
		int32_t L_7;
		L_7 = InterfaceFuncInvoker5< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int32_t, int32_t*, int32_t* >::Invoke(124, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_3, L_4, L_5, L_6);
		return L_7;
	}
}
// Method Definition Index: 117425
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_set_authorizer_m5DA23A70DA0E64D34F3DBAEBBC59C19B7A76CFFA (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* ___1_f, RuntimeObject* ___2_user_data, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* L_2 = ___1_f;
		RuntimeObject* L_3 = ___2_user_data;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker3< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933*, RuntimeObject* >::Invoke(126, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2, L_3);
		return L_4;
	}
}
// Method Definition Index: 117426
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_set_authorizer_mE7912D7FCAC61603C3770C048CE4A6DE557F0CC5 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* ___1_f, RuntimeObject* ___2_user_data, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass429_0_U3Csqlite3_set_authorizerU3Eb__0_m134F19636E67D69A3A7841EEA84C02D1E5FCA1D5_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&U3CU3Ec__DisplayClass429_0_tF649DDD522F0D50321341692A00F5965CC3248D6_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	U3CU3Ec__DisplayClass429_0_tF649DDD522F0D50321341692A00F5965CC3248D6* V_0 = NULL;
	delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* V_1 = NULL;
	{
		U3CU3Ec__DisplayClass429_0_tF649DDD522F0D50321341692A00F5965CC3248D6* L_0 = (U3CU3Ec__DisplayClass429_0_tF649DDD522F0D50321341692A00F5965CC3248D6*)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass429_0_tF649DDD522F0D50321341692A00F5965CC3248D6_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass429_0__ctor_m077C7CDEAC013F152AD5A4380DAC6A282842CDF5(L_0, NULL);
		V_0 = L_0;
		U3CU3Ec__DisplayClass429_0_tF649DDD522F0D50321341692A00F5965CC3248D6* L_1 = V_0;
		strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* L_2 = ___1_f;
		NullCheck(L_1);
		L_1->___f = L_2;
		Il2CppCodeGenWriteBarrier((void**)(&L_1->___f), (void*)L_2);
		U3CU3Ec__DisplayClass429_0_tF649DDD522F0D50321341692A00F5965CC3248D6* L_3 = V_0;
		NullCheck(L_3);
		strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* L_4 = L_3->___f;
		if (L_4)
		{
			goto IL_0019;
		}
	}
	{
		V_1 = (delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933*)NULL;
		goto IL_0026;
	}

IL_0019:
	{
		U3CU3Ec__DisplayClass429_0_tF649DDD522F0D50321341692A00F5965CC3248D6* L_5 = V_0;
		delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* L_6 = (delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933*)il2cpp_codegen_object_new(delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933_il2cpp_TypeInfo_var);
		delegate_authorizer__ctor_m4F3DB7EAF151377921A14384AB7CB7E379B497E2(L_6, L_5, (intptr_t)((void*)U3CU3Ec__DisplayClass429_0_U3Csqlite3_set_authorizerU3Eb__0_m134F19636E67D69A3A7841EEA84C02D1E5FCA1D5_RuntimeMethod_var), NULL);
		V_1 = L_6;
	}

IL_0026:
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_7 = ___0_db;
		delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* L_8 = V_1;
		RuntimeObject* L_9 = ___2_user_data;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_10;
		L_10 = raw_sqlite3_set_authorizer_m5DA23A70DA0E64D34F3DBAEBBC59C19B7A76CFFA(L_7, L_8, L_9, NULL);
		return L_10;
	}
}
// Method Definition Index: 117427
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_win32_set_directory_m6875FD8B7C89FC2071C44FE2586A8EFD0F0719CB (int32_t ___0_typ, String_t* ___1_path, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		int32_t L_1 = ___0_typ;
		String_t* L_2 = ___1_path;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_2, NULL);
		NullCheck(L_0);
		int32_t L_4;
		L_4 = InterfaceFuncInvoker2< int32_t, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(146, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_3);
		return L_4;
	}
}
// Method Definition Index: 117428
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_keyword_count_mF7496A66E62872042DDC92144E0789D074C7BEB5 (const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		NullCheck(L_0);
		int32_t L_1;
		L_1 = InterfaceFuncInvoker0< int32_t >::Invoke(147, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0);
		return L_1;
	}
}
// Method Definition Index: 117429
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_keyword_name_m5582E02D61F6EC81FC36449C7FDABE015231BDB0 (int32_t ___0_i, String_t** ___1_name, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		int32_t L_1 = ___0_i;
		String_t** L_2 = ___1_name;
		NullCheck(L_0);
		int32_t L_3;
		L_3 = InterfaceFuncInvoker2< int32_t, int32_t, String_t** >::Invoke(148, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_2);
		return L_3;
	}
}
// Method Definition Index: 117430
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t raw_sqlite3_malloc_mE56C0DC312A5C5E049247530E38AA329BD751159 (int32_t ___0_n, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		int32_t L_1 = ___0_n;
		NullCheck(L_0);
		intptr_t L_2;
		L_2 = InterfaceFuncInvoker1< intptr_t, int32_t >::Invoke(129, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117431
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t raw_sqlite3_malloc64_m8D4AF28111F5B514574663BD1F4BC6976B0932F4 (int64_t ___0_n, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		int64_t L_1 = ___0_n;
		NullCheck(L_0);
		intptr_t L_2;
		L_2 = InterfaceFuncInvoker1< intptr_t, int64_t >::Invoke(130, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return L_2;
	}
}
// Method Definition Index: 117432
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void raw_sqlite3_free_m24A2A2335CD33047C82121E325F8FB185F06CD88 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		intptr_t L_1 = ___0_p;
		NullCheck(L_0);
		InterfaceActionInvoker1< intptr_t >::Invoke(131, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1);
		return;
	}
}
// Method Definition Index: 117433
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t raw_sqlite3_serialize_m79DD1F3D9D46219483A7912465663FC07A733BFC (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_schema, int64_t* ___2_size, int32_t ___3_flags, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		String_t* L_2 = ___1_schema;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_2, NULL);
		int64_t* L_4 = ___2_size;
		int32_t L_5 = ___3_flags;
		NullCheck(L_0);
		intptr_t L_6;
		L_6 = InterfaceFuncInvoker4< intptr_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t*, int32_t >::Invoke(149, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_3, L_4, L_5);
		return L_6;
	}
}
// Method Definition Index: 117434
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t raw_sqlite3_deserialize_mA09D22DBF538057D1B34AB4A80F166C2FA9554E4 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___0_db, String_t* ___1_schema, intptr_t ___2_data, int64_t ___3_deserializedDataSize, int64_t ___4_maxDataSize, int32_t ___5_flags, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		RuntimeObject* L_0;
		L_0 = raw_get_Provider_mA15EEB776574FE94A7AC5ECA3ABFBA99EAFDE867(NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = ___0_db;
		String_t* L_2 = ___1_schema;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		L_3 = util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48(L_2, NULL);
		intptr_t L_4 = ___2_data;
		int64_t L_5 = ___3_deserializedDataSize;
		int64_t L_6 = ___4_maxDataSize;
		int32_t L_7 = ___5_flags;
		NullCheck(L_0);
		int32_t L_8;
		L_8 = InterfaceFuncInvoker6< int32_t, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, intptr_t, int64_t, int64_t, int32_t >::Invoke(150, ISQLite3Provider_t12D6FDDE2C734FC4554FA90D705E79012B3BC5E7_il2cpp_TypeInfo_var, L_0, L_1, L_3, L_4, L_5, L_6, L_7);
		return L_8;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117435
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass263_0__ctor_mA0C4F9F9AEE43DFA9511F900E58F672A0208F64A (U3CU3Ec__DisplayClass263_0_t9EA15DBB375982C7857A45FFC12C73F0B4238EB9* __this, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		return;
	}
}
// Method Definition Index: 117436
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass263_0_U3Csqlite3_config_logU3Eb__0_m8B4286DB43D892CCAC433BFC538CE4D0117CC508 (U3CU3Ec__DisplayClass263_0_t9EA15DBB375982C7857A45FFC12C73F0B4238EB9* __this, RuntimeObject* ___0_ob, int32_t ___1_e, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_msg, const RuntimeMethod* method) 
{
	{
		strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* L_0 = __this->___f;
		RuntimeObject* L_1 = ___0_ob;
		int32_t L_2 = ___1_e;
		String_t* L_3;
		L_3 = utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A((&___2_msg), NULL);
		NullCheck(L_0);
		strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_inline(L_0, L_1, L_2, L_3, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117437
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass269_0__ctor_m2C09696A2953271F2CEB026027FB892C3EF47178 (U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF* __this, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		return;
	}
}
// Method Definition Index: 117438
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass269_0_U3Csqlite3_traceU3Eb__0_m3D7C6C768B76831B199A3DEEB4508F55616E75EC (U3CU3Ec__DisplayClass269_0_tE1E5E268D24254ED5E372DB23B165B61DF4AC1BF* __this, RuntimeObject* ___0_ob, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_sp, const RuntimeMethod* method) 
{
	{
		strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* L_0 = __this->___f;
		RuntimeObject* L_1 = __this->___v;
		String_t* L_2;
		L_2 = utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A((&___1_sp), NULL);
		NullCheck(L_0);
		strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_inline(L_0, L_1, L_2, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117439
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass271_0__ctor_mD621FC47AF62F2940A6152CDA5BA7F8FC5CE324C (U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F* __this, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		return;
	}
}
// Method Definition Index: 117440
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass271_0_U3Csqlite3_profileU3Eb__0_mC0125502FE73FA12ED8241058AA31E17A35BD0C7 (U3CU3Ec__DisplayClass271_0_t5E51327338A4AF6863783DAE4054C5E61C7FE44F* __this, RuntimeObject* ___0_ob, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_sp, int64_t ___2_ns, const RuntimeMethod* method) 
{
	{
		strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* L_0 = __this->___f;
		RuntimeObject* L_1 = __this->___v;
		String_t* L_2;
		L_2 = utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A((&___1_sp), NULL);
		int64_t L_3 = ___2_ns;
		NullCheck(L_0);
		strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_inline(L_0, L_1, L_2, L_3, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117441
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass274_0__ctor_m6B059B3B06DFD76E37692FC3BCC6DAA722C29C3C (U3CU3Ec__DisplayClass274_0_t75B1EC8A8B2CD9CFE76F0CDD3507D686F9C95A31* __this, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		return;
	}
}
// Method Definition Index: 117442
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass274_0_U3Csqlite3_update_hookU3Eb__0_m9A7EB3C9B70FD4005DD270F143CDE0D73F91C033 (U3CU3Ec__DisplayClass274_0_t75B1EC8A8B2CD9CFE76F0CDD3507D686F9C95A31* __this, RuntimeObject* ___0_ob, int32_t ___1_typ, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_dbname, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_tbl, int64_t ___4_rowid, const RuntimeMethod* method) 
{
	{
		strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* L_0 = __this->___f;
		RuntimeObject* L_1 = ___0_ob;
		int32_t L_2 = ___1_typ;
		String_t* L_3;
		L_3 = utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A((&___2_dbname), NULL);
		String_t* L_4;
		L_4 = utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A((&___3_tbl), NULL);
		int64_t L_5 = ___4_rowid;
		NullCheck(L_0);
		strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_inline(L_0, L_1, L_2, L_3, L_4, L_5, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117443
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass275_0__ctor_mE3C57C5B8020F19D1D8240BBF41CC41C413E7838 (U3CU3Ec__DisplayClass275_0_t2E8E6521F4006ACCD7E5F26205A9BC7CBA6A4B9A* __this, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		return;
	}
}
// Method Definition Index: 117444
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t U3CU3Ec__DisplayClass275_0_U3Csqlite3_create_collationU3Eb__0_m04DCDC3168B2FE4CB267E97F21757904478F22F4 (U3CU3Ec__DisplayClass275_0_t2E8E6521F4006ACCD7E5F26205A9BC7CBA6A4B9A* __this, RuntimeObject* ___0_ob, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_s1, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_s2, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* L_0 = __this->___f;
		RuntimeObject* L_1 = ___0_ob;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_2 = ___1_s1;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		String_t* L_3;
		L_3 = raw_utf8_span_to_string_m1D3AC8DF369A2FFC9AC7B1A7E165F6B4C7234DE2(L_2, NULL);
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_4 = ___2_s2;
		String_t* L_5;
		L_5 = raw_utf8_span_to_string_m1D3AC8DF369A2FFC9AC7B1A7E165F6B4C7234DE2(L_4, NULL);
		NullCheck(L_0);
		int32_t L_6;
		L_6 = strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_inline(L_0, L_1, L_3, L_5, NULL);
		return L_6;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117445
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass332_0__ctor_m384555F116520FA2FCFE6E072CB793F3802B1934 (U3CU3Ec__DisplayClass332_0_t1173985050ED9406B43934B3B874C78753CF564A* __this, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		return;
	}
}
// Method Definition Index: 117446
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t U3CU3Ec__DisplayClass332_0_U3Csqlite3_execU3Eb__0_m09F33029DA46B08016391B82A5E88D8D6CF03F17 (U3CU3Ec__DisplayClass332_0_t1173985050ED9406B43934B3B874C78753CF564A* __this, RuntimeObject* ___0_ob, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___1_values, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___2_names, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* V_0 = NULL;
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* V_1 = NULL;
	int32_t V_2 = 0;
	{
		IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* L_0 = ___1_values;
		NullCheck(L_0);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_1 = (StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*)(StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*)SZArrayNew(StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248_il2cpp_TypeInfo_var, (uint32_t)((int32_t)(((RuntimeArray*)L_0)->max_length)));
		V_0 = L_1;
		IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* L_2 = ___2_names;
		NullCheck(L_2);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_3 = (StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*)(StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*)SZArrayNew(StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248_il2cpp_TypeInfo_var, (uint32_t)((int32_t)(((RuntimeArray*)L_2)->max_length)));
		V_1 = L_3;
		V_2 = 0;
		goto IL_0030;
	}

IL_0016:
	{
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_4 = V_0;
		int32_t L_5 = V_2;
		IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* L_6 = ___1_values;
		int32_t L_7 = V_2;
		NullCheck(L_6);
		int32_t L_8 = L_7;
		intptr_t L_9 = (L_6)->GetAt(static_cast<il2cpp_array_size_t>(L_8));
		String_t* L_10;
		L_10 = util_from_utf8_z_mA4AD3FF9FAB5CA653DD808B6B17A04FDF7425743(L_9, NULL);
		NullCheck(L_4);
		(L_4)->SetAt(static_cast<il2cpp_array_size_t>(L_5), (String_t*)L_10);
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_11 = V_1;
		int32_t L_12 = V_2;
		IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* L_13 = ___2_names;
		int32_t L_14 = V_2;
		NullCheck(L_13);
		int32_t L_15 = L_14;
		intptr_t L_16 = (L_13)->GetAt(static_cast<il2cpp_array_size_t>(L_15));
		String_t* L_17;
		L_17 = util_from_utf8_z_mA4AD3FF9FAB5CA653DD808B6B17A04FDF7425743(L_16, NULL);
		NullCheck(L_11);
		(L_11)->SetAt(static_cast<il2cpp_array_size_t>(L_12), (String_t*)L_17);
		int32_t L_18 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add(L_18, 1));
	}

IL_0030:
	{
		int32_t L_19 = V_2;
		IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* L_20 = ___1_values;
		NullCheck(L_20);
		if ((((int32_t)L_19) < ((int32_t)((int32_t)(((RuntimeArray*)L_20)->max_length)))))
		{
			goto IL_0016;
		}
	}
	{
		strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* L_21 = __this->___callback;
		RuntimeObject* L_22 = ___0_ob;
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_23 = V_0;
		StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* L_24 = V_1;
		NullCheck(L_21);
		int32_t L_25;
		L_25 = strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_inline(L_21, L_22, L_23, L_24, NULL);
		return L_25;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117447
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass429_0__ctor_m077C7CDEAC013F152AD5A4380DAC6A282842CDF5 (U3CU3Ec__DisplayClass429_0_tF649DDD522F0D50321341692A00F5965CC3248D6* __this, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		return;
	}
}
// Method Definition Index: 117448
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t U3CU3Ec__DisplayClass429_0_U3Csqlite3_set_authorizerU3Eb__0_m134F19636E67D69A3A7841EEA84C02D1E5FCA1D5 (U3CU3Ec__DisplayClass429_0_tF649DDD522F0D50321341692A00F5965CC3248D6* __this, RuntimeObject* ___0_ob, int32_t ___1_a, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_p0, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_p1, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___4_dbname, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___5_v, const RuntimeMethod* method) 
{
	{
		strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* L_0 = __this->___f;
		RuntimeObject* L_1 = ___0_ob;
		int32_t L_2 = ___1_a;
		String_t* L_3;
		L_3 = utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A((&___2_p0), NULL);
		String_t* L_4;
		L_4 = utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A((&___3_p1), NULL);
		String_t* L_5;
		L_5 = utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A((&___4_dbname), NULL);
		String_t* L_6;
		L_6 = utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A((&___5_v), NULL);
		NullCheck(L_0);
		int32_t L_7;
		L_7 = strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_inline(L_0, L_1, L_2, L_3, L_4, L_5, L_6, NULL);
		return L_7;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117449
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_backup__ctor_m63CDC567578C17953C38ACA2E9FAAE0BFF778F7C (sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* __this, const RuntimeMethod* method) 
{
	{
		SafeHandle__ctor_m23E44C94503043292DCD4E87818082CFC09A7F4B(__this, 0, (bool)1, NULL);
		return;
	}
}
// Method Definition Index: 117450
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* sqlite3_backup_From_m7D3819E372FD7E6026733E16F9F21DE0BEA08775 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* L_0 = (sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30*)il2cpp_codegen_object_new(sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30_il2cpp_TypeInfo_var);
		sqlite3_backup__ctor_m63CDC567578C17953C38ACA2E9FAAE0BFF778F7C(L_0, NULL);
		sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* L_1 = L_0;
		intptr_t L_2 = ___0_p;
		NullCheck(L_1);
		SafeHandle_SetHandle_m003D64748F9DFBA1E3C0B23798C23BA81AA21C2A_inline(L_1, L_2, NULL);
		return L_1;
	}
}
// Method Definition Index: 117451
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool sqlite3_backup_get_IsInvalid_m8C31716D1F4164DA473615E60276C10E3DD483AA (sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* __this, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		bool L_1;
		L_1 = IntPtr_op_Equality_m7D9CDCDE9DC2A0C2C614633F4921E90187FAB271_inline(L_0, 0, NULL);
		return L_1;
	}
}
// Method Definition Index: 117452
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool sqlite3_backup_ReleaseHandle_m86563AF4C898B4AF3D7BB1A02CD9BF116B18893B (sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_1;
		L_1 = raw_internal_sqlite3_backup_finish_m84D9D7C6128794ADADBF3757E4587590A8749DC1(L_0, NULL);
		return (bool)1;
	}
}
// Method Definition Index: 117453
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t sqlite3_backup_manual_close_m16BBF17DEA8FFABBE26C4A1668815FA624897244 (sqlite3_backup_tA63020E6886E8E4AC469CE68788C7FA2BE7B8C30* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_1;
		L_1 = raw_internal_sqlite3_backup_finish_m84D9D7C6128794ADADBF3757E4587590A8749DC1(L_0, NULL);
		((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle = 0;
		return L_1;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117454
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_context__ctor_mD3A1FF371768B7E9C88EC86EFE32952B8D89D71F (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		RuntimeObject* L_0 = ___0_user_data;
		__this->____user_data = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____user_data), (void*)L_0);
		return;
	}
}
// Method Definition Index: 117455
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* sqlite3_context_get_user_data_m2B56AA945EBDDC7B032007BA993E2B0D6311E609 (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* __this, const RuntimeMethod* method) 
{
	{
		RuntimeObject* L_0 = __this->____user_data;
		return L_0;
	}
}
// Method Definition Index: 117456
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259 (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* __this, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = __this->____p;
		return L_0;
	}
}
// Method Definition Index: 117457
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_context_set_context_ptr_m75037F45D04497B679E79EF821D191289E5917A3 (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* __this, intptr_t ___0_p, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ___0_p;
		__this->____p = L_0;
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117458
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_value__ctor_m653B23B5B2873FC7E8AB166AAE5E00151A929676 (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* __this, intptr_t ___0_p, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		intptr_t L_0 = ___0_p;
		__this->____p = L_0;
		return;
	}
}
// Method Definition Index: 117459
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t sqlite3_value_get_ptr_mAF588DD3703B49533D61586741960A38A4515AD9 (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* __this, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = __this->____p;
		return L_0;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117460
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_blob__ctor_mF7C7B725FB9FE141587C4BE5A547765610962159 (sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* __this, const RuntimeMethod* method) 
{
	{
		SafeHandle__ctor_m23E44C94503043292DCD4E87818082CFC09A7F4B(__this, 0, (bool)1, NULL);
		return;
	}
}
// Method Definition Index: 117461
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* sqlite3_blob_From_mB4297A66CCFA501D59A8B2C12BDF17FEB56E4FE0 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* L_0 = (sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057*)il2cpp_codegen_object_new(sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057_il2cpp_TypeInfo_var);
		sqlite3_blob__ctor_mF7C7B725FB9FE141587C4BE5A547765610962159(L_0, NULL);
		sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* L_1 = L_0;
		intptr_t L_2 = ___0_p;
		NullCheck(L_1);
		SafeHandle_SetHandle_m003D64748F9DFBA1E3C0B23798C23BA81AA21C2A_inline(L_1, L_2, NULL);
		return L_1;
	}
}
// Method Definition Index: 117462
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool sqlite3_blob_get_IsInvalid_mE9291E4818E97885BC9F83E0BB74626AB759CAB5 (sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* __this, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		bool L_1;
		L_1 = IntPtr_op_Equality_m7D9CDCDE9DC2A0C2C614633F4921E90187FAB271_inline(L_0, 0, NULL);
		return L_1;
	}
}
// Method Definition Index: 117463
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool sqlite3_blob_ReleaseHandle_m9B282376A7087BECD8B498F554D05441472093F1 (sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_1;
		L_1 = raw_internal_sqlite3_blob_close_mF4D383511A58F4241E7337A440194216B4505DEC(L_0, NULL);
		return (bool)1;
	}
}
// Method Definition Index: 117464
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t sqlite3_blob_manual_close_m32537CD8CDCC89D1CBAD7832CFA559A05C61BA5F (sqlite3_blob_tFE15C3078D13F18E37D20A3903D8198FD540C057* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_1;
		L_1 = raw_internal_sqlite3_blob_close_mF4D383511A58F4241E7337A440194216B4505DEC(L_0, NULL);
		((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle = 0;
		return L_1;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117465
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_snapshot__ctor_m6CAB94F95FF7CC8925154B4E452ECFD6F16FD6C7 (sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* __this, const RuntimeMethod* method) 
{
	{
		SafeHandle__ctor_m23E44C94503043292DCD4E87818082CFC09A7F4B(__this, 0, (bool)1, NULL);
		return;
	}
}
// Method Definition Index: 117466
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* sqlite3_snapshot_From_mFC6D90D41F3A68F30767F7A792A370CFF7D39E83 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* L_0 = (sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475*)il2cpp_codegen_object_new(sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475_il2cpp_TypeInfo_var);
		sqlite3_snapshot__ctor_m6CAB94F95FF7CC8925154B4E452ECFD6F16FD6C7(L_0, NULL);
		sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* L_1 = L_0;
		intptr_t L_2 = ___0_p;
		NullCheck(L_1);
		SafeHandle_SetHandle_m003D64748F9DFBA1E3C0B23798C23BA81AA21C2A_inline(L_1, L_2, NULL);
		return L_1;
	}
}
// Method Definition Index: 117467
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool sqlite3_snapshot_get_IsInvalid_m9C57C795BF1A7E408DE471EFB597474BBD848236 (sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* __this, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		bool L_1;
		L_1 = IntPtr_op_Equality_m7D9CDCDE9DC2A0C2C614633F4921E90187FAB271_inline(L_0, 0, NULL);
		return L_1;
	}
}
// Method Definition Index: 117468
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool sqlite3_snapshot_ReleaseHandle_m9C2A08C206B3A495107DE0FD35E9BA4BD892AA59 (sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		raw_internal_sqlite3_snapshot_free_mB9410BEFC8368B7ECE442E6CB7D8725A51649FD3(L_0, NULL);
		return (bool)1;
	}
}
// Method Definition Index: 117469
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_snapshot_manual_close_m49FE953A1E7ACF28AA2C4A2C7EF8DEC9A953D9C8 (sqlite3_snapshot_tABC1706F945F4C8DF190B74C4DD0C0FC64682475* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		raw_internal_sqlite3_snapshot_free_mB9410BEFC8368B7ECE442E6CB7D8725A51649FD3(L_0, NULL);
		((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle = 0;
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117470
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* sqlite3_stmt_From_mA7A4E31058B72E4386C4B74F4B1A93BF446CC207 (intptr_t ___0_p, sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* ___1_db, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* V_0 = NULL;
	{
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_0 = (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F*)il2cpp_codegen_object_new(sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F_il2cpp_TypeInfo_var);
		sqlite3_stmt__ctor_m8274DE84D8B25F50A07FA11BDB40CA18880774B3(L_0, NULL);
		V_0 = L_0;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_1 = V_0;
		intptr_t L_2 = ___0_p;
		NullCheck(L_1);
		SafeHandle_SetHandle_m003D64748F9DFBA1E3C0B23798C23BA81AA21C2A_inline(L_1, L_2, NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_3 = ___1_db;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_4 = V_0;
		NullCheck(L_3);
		sqlite3_add_stmt_mD2483B53A00B53DE52B70C40C475EC191199C73B(L_3, L_4, NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_5 = V_0;
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_6 = ___1_db;
		NullCheck(L_5);
		L_5->____db = L_6;
		Il2CppCodeGenWriteBarrier((void**)(&L_5->____db), (void*)L_6);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_7 = V_0;
		return L_7;
	}
}
// Method Definition Index: 117471
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_stmt__ctor_m8274DE84D8B25F50A07FA11BDB40CA18880774B3 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* __this, const RuntimeMethod* method) 
{
	{
		SafeHandle__ctor_m23E44C94503043292DCD4E87818082CFC09A7F4B(__this, 0, (bool)1, NULL);
		return;
	}
}
// Method Definition Index: 117472
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool sqlite3_stmt_get_IsInvalid_m6E1050B83C665C3802FA731D0FDB8B6041BB5E53 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* __this, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		bool L_1;
		L_1 = IntPtr_op_Equality_m7D9CDCDE9DC2A0C2C614633F4921E90187FAB271_inline(L_0, 0, NULL);
		return L_1;
	}
}
// Method Definition Index: 117473
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool sqlite3_stmt_ReleaseHandle_mF833B591DA1E47FA9D5565BBB11370820582C4E8 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_1;
		L_1 = raw_internal_sqlite3_finalize_m4A8747B9480EA6234BC79A647F029851A3A99A41(L_0, NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_2 = __this->____db;
		NullCheck(L_2);
		sqlite3_remove_stmt_m4388870B666E82A70AE88284E79AADD62C321BA6(L_2, __this, NULL);
		return (bool)1;
	}
}
// Method Definition Index: 117474
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t sqlite3_stmt_manual_close_m3A3C2F6C6782CB371A60EA7B5372F9CCA721577A (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_1;
		L_1 = raw_internal_sqlite3_finalize_m4A8747B9480EA6234BC79A647F029851A3A99A41(L_0, NULL);
		((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle = 0;
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_2 = __this->____db;
		NullCheck(L_2);
		sqlite3_remove_stmt_m4388870B666E82A70AE88284E79AADD62C321BA6(L_2, __this, NULL);
		return L_1;
	}
}
// Method Definition Index: 117475
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t sqlite3_stmt_get_ptr_mF5030B60EB110512D248A632A61AE2F899580B58 (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* __this, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		return L_0;
	}
}
// Method Definition Index: 117476
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* sqlite3_stmt_get_db_m31DCFD46B918941ED21CAD5F953201B59EF259CF (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* __this, const RuntimeMethod* method) 
{
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_0 = __this->____db;
		return L_0;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117477
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3__ctor_m9665E7B91BF88357F8083A085B576DCC73A40B93 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, const RuntimeMethod* method) 
{
	{
		SafeHandle__ctor_m23E44C94503043292DCD4E87818082CFC09A7F4B(__this, 0, (bool)1, NULL);
		return;
	}
}
// Method Definition Index: 117478
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool sqlite3_get_IsInvalid_m91F60F4362294FF1FE48934A7EB645956C6F164D (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		bool L_1;
		L_1 = IntPtr_op_Equality_m7D9CDCDE9DC2A0C2C614633F4921E90187FAB271_inline(L_0, 0, NULL);
		return L_1;
	}
}
// Method Definition Index: 117479
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool sqlite3_ReleaseHandle_m55595E85FCF053A51BDD4AD6B8B7A3BDA48AF275 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_1;
		L_1 = raw_internal_sqlite3_close_v2_m50AD68E0E1ED21F1E91ECF9E2F7D9FD360B18A9D(L_0, NULL);
		sqlite3_dispose_extra_m353F2CCE56ADF182709E81FEEEC40854E41F807B(__this, NULL);
		return (bool)1;
	}
}
// Method Definition Index: 117480
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t sqlite3_manual_close_v2_m9913465FCFAA3E200B65FC3E7396D23B00D5351C (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_1;
		L_1 = raw_internal_sqlite3_close_v2_m50AD68E0E1ED21F1E91ECF9E2F7D9FD360B18A9D(L_0, NULL);
		((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle = 0;
		sqlite3_dispose_extra_m353F2CCE56ADF182709E81FEEEC40854E41F807B(__this, NULL);
		return L_1;
	}
}
// Method Definition Index: 117481
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t sqlite3_manual_close_m385D8CE62C53FB890E5878C8FC353A95485514CC (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		il2cpp_codegen_runtime_class_init_inline(raw_t69949A70FEB6A3380DD174C22371DD56C26EE95C_il2cpp_TypeInfo_var);
		int32_t L_1;
		L_1 = raw_internal_sqlite3_close_m1B069EFBA4CBDA60B0522F50E95833EE8E2F8C10(L_0, NULL);
		((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle = 0;
		sqlite3_dispose_extra_m353F2CCE56ADF182709E81FEEEC40854E41F807B(__this, NULL);
		return L_1;
	}
}
// Method Definition Index: 117482
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* sqlite3_New_m5AFB69A71B456CA905E9400C0D8F2A059D8E74B3 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_0 = (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2*)il2cpp_codegen_object_new(sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2_il2cpp_TypeInfo_var);
		sqlite3__ctor_m9665E7B91BF88357F8083A085B576DCC73A40B93(L_0, NULL);
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_1 = L_0;
		intptr_t L_2 = ___0_p;
		NullCheck(L_1);
		SafeHandle_SetHandle_m003D64748F9DFBA1E3C0B23798C23BA81AA21C2A_inline(L_1, L_2, NULL);
		return L_1;
	}
}
// Method Definition Index: 117483
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_enable_sqlite3_next_stmt_m8838A7FDEE7942774B115E5CD1C4E9660249E42A (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, bool ___0_enabled, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2__ctor_m738B333B82DBBF304A15728C98883F5163204374_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		bool L_0 = ___0_enabled;
		if (!L_0)
		{
			goto IL_0017;
		}
	}
	{
		ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476* L_1 = __this->____stmts;
		if (L_1)
		{
			goto IL_001e;
		}
	}
	{
		ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476* L_2 = (ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476*)il2cpp_codegen_object_new(ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476_il2cpp_TypeInfo_var);
		ConcurrentDictionary_2__ctor_m738B333B82DBBF304A15728C98883F5163204374(L_2, ConcurrentDictionary_2__ctor_m738B333B82DBBF304A15728C98883F5163204374_RuntimeMethod_var);
		__this->____stmts = L_2;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____stmts), (void*)L_2);
		return;
	}

IL_0017:
	{
		__this->____stmts = (ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476*)NULL;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____stmts), (void*)(ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476*)NULL);
	}

IL_001e:
	{
		return;
	}
}
// Method Definition Index: 117484
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_add_stmt_mD2483B53A00B53DE52B70C40C475EC191199C73B (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_stmt, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2_set_Item_m0B68193309F23091AEE40F02FBB000FAFB452DF8_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476* L_0 = __this->____stmts;
		if (!L_0)
		{
			goto IL_001a;
		}
	}
	{
		ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476* L_1 = __this->____stmts;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_2 = ___0_stmt;
		NullCheck(L_2);
		intptr_t L_3;
		L_3 = sqlite3_stmt_get_ptr_mF5030B60EB110512D248A632A61AE2F899580B58_inline(L_2, NULL);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_4 = ___0_stmt;
		NullCheck(L_1);
		ConcurrentDictionary_2_set_Item_m0B68193309F23091AEE40F02FBB000FAFB452DF8(L_1, L_3, L_4, ConcurrentDictionary_2_set_Item_m0B68193309F23091AEE40F02FBB000FAFB452DF8_RuntimeMethod_var);
	}

IL_001a:
	{
		return;
	}
}
// Method Definition Index: 117485
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* sqlite3_find_stmt_m6DB56FD791338E8E5524252CE51E90D7D6624FE3 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2_get_Item_mE58842628CBB5BB71F07874570EC30E7FC721445_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476* L_0 = __this->____stmts;
		if (!L_0)
		{
			goto IL_0015;
		}
	}
	{
		ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476* L_1 = __this->____stmts;
		intptr_t L_2 = ___0_p;
		NullCheck(L_1);
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_3;
		L_3 = ConcurrentDictionary_2_get_Item_mE58842628CBB5BB71F07874570EC30E7FC721445(L_1, L_2, ConcurrentDictionary_2_get_Item_mE58842628CBB5BB71F07874570EC30E7FC721445_RuntimeMethod_var);
		return L_3;
	}

IL_0015:
	{
		Exception_t* L_4 = (Exception_t*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Exception_t_il2cpp_TypeInfo_var)));
		Exception__ctor_m9B2BD92CD68916245A75109105D9071C9D430E7F(L_4, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral929AB203C6C048DBA2C6EA10E47D89A2FDE3F41A)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_4, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&sqlite3_find_stmt_m6DB56FD791338E8E5524252CE51E90D7D6624FE3_RuntimeMethod_var)));
	}
}
// Method Definition Index: 117486
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_remove_stmt_m4388870B666E82A70AE88284E79AADD62C321BA6 (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* ___0_s, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2_TryRemove_m8C588451D72CA78D8618557F8A75D35324158B5D_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* V_0 = NULL;
	{
		ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476* L_0 = __this->____stmts;
		if (!L_0)
		{
			goto IL_001c;
		}
	}
	{
		ConcurrentDictionary_2_tEFA4BFFF7CD4FFD4A7A9EF772AF343FBCFABB476* L_1 = __this->____stmts;
		sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* L_2 = ___0_s;
		NullCheck(L_2);
		intptr_t L_3;
		L_3 = sqlite3_stmt_get_ptr_mF5030B60EB110512D248A632A61AE2F899580B58_inline(L_2, NULL);
		NullCheck(L_1);
		bool L_4;
		L_4 = ConcurrentDictionary_2_TryRemove_m8C588451D72CA78D8618557F8A75D35324158B5D(L_1, L_3, (&V_0), ConcurrentDictionary_2_TryRemove_m8C588451D72CA78D8618557F8A75D35324158B5D_RuntimeMethod_var);
	}

IL_001c:
	{
		return;
	}
}
// Method Definition Index: 117488
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void sqlite3_dispose_extra_m353F2CCE56ADF182709E81FEEEC40854E41F807B (sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* V_0 = NULL;
	{
		RuntimeObject** L_0 = (RuntimeObject**)(&__this->___extra);
		RuntimeObject* L_1;
		L_1 = InterlockedExchangeImpl<RuntimeObject*>(L_0, (RuntimeObject*)NULL);
		V_0 = L_1;
		RuntimeObject* L_2 = V_0;
		if (!L_2)
		{
			goto IL_0016;
		}
	}
	{
		RuntimeObject* L_3 = V_0;
		NullCheck(L_3);
		InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_3);
	}

IL_0016:
	{
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
int32_t delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_Multicast(delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* __this, RuntimeObject* ___0_user_data, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_s1, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_s2, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	int32_t retVal = 0;
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* currentDelegate = reinterpret_cast<delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF*>(delegatesToInvoke[i]);
		typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, const RuntimeMethod*);
		retVal = ((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, ___1_s1, ___2_s2, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
	return retVal;
}
int32_t delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_OpenInst(delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* __this, RuntimeObject* ___0_user_data, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_s1, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_s2, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_s1, ___2_s2, method);
}
int32_t delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_OpenStatic(delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* __this, RuntimeObject* ___0_user_data, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_s1, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_s2, const RuntimeMethod* method)
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_s1, ___2_s2, method);
}
int32_t delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_OpenVirtual(delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* __this, RuntimeObject* ___0_user_data, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_s1, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_s2, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return VirtualFuncInvoker2< int32_t, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data, ___1_s1, ___2_s2);
}
int32_t delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_OpenInterface(delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* __this, RuntimeObject* ___0_user_data, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_s1, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_s2, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return InterfaceFuncInvoker2< int32_t, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data, ___1_s1, ___2_s2);
}
int32_t delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_OpenGenericVirtual(delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* __this, RuntimeObject* ___0_user_data, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_s1, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_s2, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericVirtualFuncInvoker2< int32_t, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D >::Invoke(method, ___0_user_data, ___1_s1, ___2_s2);
}
int32_t delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_OpenGenericInterface(delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* __this, RuntimeObject* ___0_user_data, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_s1, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_s2, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericInterfaceFuncInvoker2< int32_t, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D >::Invoke(method, ___0_user_data, ___1_s1, ___2_s2);
}
// Method Definition Index: 117489
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_collation__ctor_mF884888642B63BD696BC37E898161379BDAA85AB (delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 3;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 2;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_Multicast;
}
// Method Definition Index: 117490
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8 (delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* __this, RuntimeObject* ___0_user_data, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_s1, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_s2, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_s1, ___2_s2, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117491
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* delegate_collation_BeginInvoke_mA3CF8AD2B82317FFEAD4200841EDFAABD53EB081 (delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* __this, RuntimeObject* ___0_user_data, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_s1, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_s2, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___3_callback, RuntimeObject* ___4_object, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[4] = {0};
	__d_args[0] = ___0_user_data;
	__d_args[1] = Box(ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D_il2cpp_TypeInfo_var, &___1_s1);
	__d_args[2] = Box(ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D_il2cpp_TypeInfo_var, &___2_s2);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___3_callback, (RuntimeObject*)___4_object);
}
// Method Definition Index: 117492
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t delegate_collation_EndInvoke_m8FFD2C306858A28147F3E9B444F22CEF2DDBD7D8 (delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
	return *(int32_t*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
void delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_Multicast(delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_database, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_table, int64_t ___4_rowid, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* currentDelegate = reinterpret_cast<delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80*>(delegatesToInvoke[i]);
		typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t, const RuntimeMethod*);
		((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
}
void delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_OpenInst(delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_database, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_table, int64_t ___4_rowid, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef void (*FunctionPointerType) (RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid, method);
}
void delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_OpenStatic(delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_database, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_table, int64_t ___4_rowid, const RuntimeMethod* method)
{
	typedef void (*FunctionPointerType) (RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid, method);
}
void delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_OpenVirtual(delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_database, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_table, int64_t ___4_rowid, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	VirtualActionInvoker4< int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid);
}
void delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_OpenInterface(delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_database, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_table, int64_t ___4_rowid, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	InterfaceActionInvoker4< int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid);
}
void delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_OpenGenericVirtual(delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_database, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_table, int64_t ___4_rowid, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericVirtualActionInvoker4< int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t >::Invoke(method, ___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid);
}
void delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_OpenGenericInterface(delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_database, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_table, int64_t ___4_rowid, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericInterfaceActionInvoker4< int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t >::Invoke(method, ___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid);
}
// Method Definition Index: 117493
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_update__ctor_m17032A8DE0B56F6F7EEB38252AA6FF345689EE93 (delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 5;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 4;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_Multicast;
}
// Method Definition Index: 117494
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349 (delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_database, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_table, int64_t ___4_rowid, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117495
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* delegate_update_BeginInvoke_mDC20A7FB62F01385608B87534BCFDE741F5E7B3A (delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_database, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_table, int64_t ___4_rowid, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___5_callback, RuntimeObject* ___6_object, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[6] = {0};
	__d_args[0] = ___0_user_data;
	__d_args[1] = Box(il2cpp_defaults.int32_class, &___1_type);
	__d_args[2] = Box(utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var, &___2_database);
	__d_args[3] = Box(utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var, &___3_table);
	__d_args[4] = Box(il2cpp_defaults.int64_class, &___4_rowid);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___5_callback, (RuntimeObject*)___6_object);
}
// Method Definition Index: 117496
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_update_EndInvoke_m47053E6CF01A17695147C3A487B3AACC47EFA528 (delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
void delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_Multicast(delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_msg, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* currentDelegate = reinterpret_cast<delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712*>(delegatesToInvoke[i]);
		typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
		((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, ___1_errorCode, ___2_msg, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
}
void delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_OpenInst(delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_msg, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef void (*FunctionPointerType) (RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_errorCode, ___2_msg, method);
}
void delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_OpenStatic(delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_msg, const RuntimeMethod* method)
{
	typedef void (*FunctionPointerType) (RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_errorCode, ___2_msg, method);
}
void delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_OpenVirtual(delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_msg, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	VirtualActionInvoker2< int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data, ___1_errorCode, ___2_msg);
}
void delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_OpenInterface(delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_msg, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	InterfaceActionInvoker2< int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data, ___1_errorCode, ___2_msg);
}
void delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_OpenGenericVirtual(delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_msg, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericVirtualActionInvoker2< int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(method, ___0_user_data, ___1_errorCode, ___2_msg);
}
void delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_OpenGenericInterface(delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_msg, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericInterfaceActionInvoker2< int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(method, ___0_user_data, ___1_errorCode, ___2_msg);
}
// Method Definition Index: 117497
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_log__ctor_m90FE70F302D363779BDD720CE99A7B2A6B07E02A (delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 3;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 2;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_Multicast;
}
// Method Definition Index: 117498
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19 (delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_msg, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_errorCode, ___2_msg, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117499
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* delegate_log_BeginInvoke_mCE40BA2D7C0E63D538AAD0CA80136C0869DCB1AE (delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_msg, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___3_callback, RuntimeObject* ___4_object, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[4] = {0};
	__d_args[0] = ___0_user_data;
	__d_args[1] = Box(il2cpp_defaults.int32_class, &___1_errorCode);
	__d_args[2] = Box(utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var, &___2_msg);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___3_callback, (RuntimeObject*)___4_object);
}
// Method Definition Index: 117500
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_log_EndInvoke_m6776ACD840F9BC8B70C9CD152CEA4335E8945106 (delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
int32_t delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_Multicast(delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_param0, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_param1, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___4_dbName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___5_inner_most_trigger_or_view, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	int32_t retVal = 0;
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* currentDelegate = reinterpret_cast<delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933*>(delegatesToInvoke[i]);
		typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
		retVal = ((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
	return retVal;
}
int32_t delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_OpenInst(delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_param0, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_param1, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___4_dbName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___5_inner_most_trigger_or_view, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view, method);
}
int32_t delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_OpenStatic(delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_param0, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_param1, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___4_dbName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___5_inner_most_trigger_or_view, const RuntimeMethod* method)
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view, method);
}
int32_t delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_OpenVirtual(delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_param0, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_param1, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___4_dbName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___5_inner_most_trigger_or_view, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return VirtualFuncInvoker5< int32_t, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view);
}
int32_t delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_OpenInterface(delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_param0, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_param1, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___4_dbName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___5_inner_most_trigger_or_view, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return InterfaceFuncInvoker5< int32_t, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view);
}
int32_t delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_OpenGenericVirtual(delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_param0, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_param1, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___4_dbName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___5_inner_most_trigger_or_view, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericVirtualFuncInvoker5< int32_t, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(method, ___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view);
}
int32_t delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_OpenGenericInterface(delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_param0, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_param1, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___4_dbName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___5_inner_most_trigger_or_view, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericInterfaceFuncInvoker5< int32_t, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(method, ___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view);
}
// Method Definition Index: 117501
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_authorizer__ctor_m4F3DB7EAF151377921A14384AB7CB7E379B497E2 (delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 6;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 5;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_Multicast;
}
// Method Definition Index: 117502
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F (delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_param0, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_param1, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___4_dbName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___5_inner_most_trigger_or_view, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117503
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* delegate_authorizer_BeginInvoke_m396289C95F9D4EF6384C45BBA538A0C505BA93F4 (delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_param0, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_param1, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___4_dbName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___5_inner_most_trigger_or_view, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___6_callback, RuntimeObject* ___7_object, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[7] = {0};
	__d_args[0] = ___0_user_data;
	__d_args[1] = Box(il2cpp_defaults.int32_class, &___1_action_code);
	__d_args[2] = Box(utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var, &___2_param0);
	__d_args[3] = Box(utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var, &___3_param1);
	__d_args[4] = Box(utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var, &___4_dbName);
	__d_args[5] = Box(utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var, &___5_inner_most_trigger_or_view);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___6_callback, (RuntimeObject*)___7_object);
}
// Method Definition Index: 117504
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t delegate_authorizer_EndInvoke_m533D6F2FA7DA2C1C0AD7DC023118BA36450D8D92 (delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
	return *(int32_t*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
int32_t delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_Multicast(delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* __this, RuntimeObject* ___0_user_data, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___1_values, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___2_names, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	int32_t retVal = 0;
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* currentDelegate = reinterpret_cast<delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3*>(delegatesToInvoke[i]);
		typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*, const RuntimeMethod*);
		retVal = ((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, ___1_values, ___2_names, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
	return retVal;
}
int32_t delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_OpenInst(delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* __this, RuntimeObject* ___0_user_data, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___1_values, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___2_names, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_values, ___2_names, method);
}
int32_t delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_OpenStatic(delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* __this, RuntimeObject* ___0_user_data, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___1_values, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___2_names, const RuntimeMethod* method)
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_values, ___2_names, method);
}
int32_t delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_OpenVirtual(delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* __this, RuntimeObject* ___0_user_data, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___1_values, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___2_names, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return VirtualFuncInvoker2< int32_t, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data, ___1_values, ___2_names);
}
int32_t delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_OpenInterface(delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* __this, RuntimeObject* ___0_user_data, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___1_values, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___2_names, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return InterfaceFuncInvoker2< int32_t, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data, ___1_values, ___2_names);
}
int32_t delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_OpenGenericVirtual(delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* __this, RuntimeObject* ___0_user_data, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___1_values, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___2_names, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericVirtualFuncInvoker2< int32_t, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* >::Invoke(method, ___0_user_data, ___1_values, ___2_names);
}
int32_t delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_OpenGenericInterface(delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* __this, RuntimeObject* ___0_user_data, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___1_values, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___2_names, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericInterfaceFuncInvoker2< int32_t, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* >::Invoke(method, ___0_user_data, ___1_values, ___2_names);
}
// Method Definition Index: 117505
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_exec__ctor_m64682E5DA3B842E47174CB722B43977DFCF5AE5B (delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 3;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 2;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_Multicast;
}
// Method Definition Index: 117506
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B (delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* __this, RuntimeObject* ___0_user_data, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___1_values, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___2_names, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_values, ___2_names, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117507
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* delegate_exec_BeginInvoke_mDC7EFD9D2F45A087FD28EDA0559138059E7DF30E (delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* __this, RuntimeObject* ___0_user_data, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___1_values, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___2_names, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___3_callback, RuntimeObject* ___4_object, const RuntimeMethod* method) 
{
	void *__d_args[4] = {0};
	__d_args[0] = ___0_user_data;
	__d_args[1] = ___1_values;
	__d_args[2] = ___2_names;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___3_callback, (RuntimeObject*)___4_object);
}
// Method Definition Index: 117508
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t delegate_exec_EndInvoke_m90B0C488FD304510501E416F34F5F6F48092599D (delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
	return *(int32_t*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
int32_t delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_Multicast(delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	int32_t retVal = 0;
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* currentDelegate = reinterpret_cast<delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A*>(delegatesToInvoke[i]);
		typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, const RuntimeMethod*);
		retVal = ((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
	return retVal;
}
int32_t delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_OpenInst(delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, method);
}
int32_t delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_OpenStatic(delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, method);
}
int32_t delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_OpenVirtual(delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return VirtualFuncInvoker0< int32_t >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data);
}
int32_t delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_OpenInterface(delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return InterfaceFuncInvoker0< int32_t >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data);
}
int32_t delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_OpenGenericVirtual(delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericVirtualFuncInvoker0< int32_t >::Invoke(method, ___0_user_data);
}
int32_t delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_OpenGenericInterface(delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericInterfaceFuncInvoker0< int32_t >::Invoke(method, ___0_user_data);
}
// Method Definition Index: 117509
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_commit__ctor_mDC224841125382D9102CB100B9AA54CD63B324AE (delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 1;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 0;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_Multicast;
}
// Method Definition Index: 117510
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8 (delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117511
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* delegate_commit_BeginInvoke_m2E4497F373ED48B4F2D4914FF24DF279E018E336 (delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* __this, RuntimeObject* ___0_user_data, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___1_callback, RuntimeObject* ___2_object, const RuntimeMethod* method) 
{
	void *__d_args[2] = {0};
	__d_args[0] = ___0_user_data;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___1_callback, (RuntimeObject*)___2_object);
}
// Method Definition Index: 117512
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t delegate_commit_EndInvoke_mCAC9CFF8DF38D44337630DABE4D4062E78663A68 (delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
	return *(int32_t*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
void delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_Multicast(delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* currentDelegate = reinterpret_cast<delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174*>(delegatesToInvoke[i]);
		typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, const RuntimeMethod*);
		((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
}
void delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_OpenInst(delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef void (*FunctionPointerType) (RuntimeObject*, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, method);
}
void delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_OpenStatic(delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	typedef void (*FunctionPointerType) (RuntimeObject*, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, method);
}
void delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_OpenVirtual(delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	VirtualActionInvoker0::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data);
}
void delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_OpenInterface(delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	InterfaceActionInvoker0::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data);
}
void delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_OpenGenericVirtual(delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericVirtualActionInvoker0::Invoke(method, ___0_user_data);
}
void delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_OpenGenericInterface(delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericInterfaceActionInvoker0::Invoke(method, ___0_user_data);
}
// Method Definition Index: 117513
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_rollback__ctor_mF1BC5B6A3F3EAF09E5B75711394F4ADA0350CAB9 (delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 1;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 0;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_Multicast;
}
// Method Definition Index: 117514
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8 (delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117515
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* delegate_rollback_BeginInvoke_mD04B01F16D8870235821758A0571AE2089CFE4EF (delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* __this, RuntimeObject* ___0_user_data, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___1_callback, RuntimeObject* ___2_object, const RuntimeMethod* method) 
{
	void *__d_args[2] = {0};
	__d_args[0] = ___0_user_data;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___1_callback, (RuntimeObject*)___2_object);
}
// Method Definition Index: 117516
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_rollback_EndInvoke_m752A1C5672B5449C29A7AA3BCB82775832FADDE7 (delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
void delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_Multicast(delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* currentDelegate = reinterpret_cast<delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172*>(delegatesToInvoke[i]);
		typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
		((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, ___1_statement, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
}
void delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_OpenInst(delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef void (*FunctionPointerType) (RuntimeObject*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_statement, method);
}
void delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_OpenStatic(delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, const RuntimeMethod* method)
{
	typedef void (*FunctionPointerType) (RuntimeObject*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_statement, method);
}
void delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_OpenVirtual(delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	VirtualActionInvoker1< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data, ___1_statement);
}
void delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_OpenInterface(delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	InterfaceActionInvoker1< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data, ___1_statement);
}
void delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_OpenGenericVirtual(delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericVirtualActionInvoker1< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(method, ___0_user_data, ___1_statement);
}
void delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_OpenGenericInterface(delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericInterfaceActionInvoker1< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 >::Invoke(method, ___0_user_data, ___1_statement);
}
// Method Definition Index: 117517
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_trace__ctor_m363F86F59BCE053BF974338A6E05C69C95C8D189 (delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 2;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 1;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_Multicast;
}
// Method Definition Index: 117518
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E (delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_statement, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117519
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* delegate_trace_BeginInvoke_m06D9F702DB373A5DFA03DC3BB5854894F2CAD81E (delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___2_callback, RuntimeObject* ___3_object, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[3] = {0};
	__d_args[0] = ___0_user_data;
	__d_args[1] = Box(utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var, &___1_statement);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___2_callback, (RuntimeObject*)___3_object);
}
// Method Definition Index: 117520
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_trace_EndInvoke_mD60F3D489B2B83AA429020D8D09C3273CB22BFBB (delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
void delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_Multicast(delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, int64_t ___2_ns, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* currentDelegate = reinterpret_cast<delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779*>(delegatesToInvoke[i]);
		typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t, const RuntimeMethod*);
		((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, ___1_statement, ___2_ns, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
}
void delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_OpenInst(delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, int64_t ___2_ns, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef void (*FunctionPointerType) (RuntimeObject*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_statement, ___2_ns, method);
}
void delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_OpenStatic(delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, int64_t ___2_ns, const RuntimeMethod* method)
{
	typedef void (*FunctionPointerType) (RuntimeObject*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_user_data, ___1_statement, ___2_ns, method);
}
void delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_OpenVirtual(delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, int64_t ___2_ns, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	VirtualActionInvoker2< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data, ___1_statement, ___2_ns);
}
void delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_OpenInterface(delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, int64_t ___2_ns, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	InterfaceActionInvoker2< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data, ___1_statement, ___2_ns);
}
void delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_OpenGenericVirtual(delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, int64_t ___2_ns, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericVirtualActionInvoker2< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t >::Invoke(method, ___0_user_data, ___1_statement, ___2_ns);
}
void delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_OpenGenericInterface(delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, int64_t ___2_ns, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	GenericInterfaceActionInvoker2< utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t >::Invoke(method, ___0_user_data, ___1_statement, ___2_ns);
}
// Method Definition Index: 117521
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_profile__ctor_mACA8FE55EFC53A98950C22BD55D8B8C0A3F6CEDA (delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 3;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 2;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_Multicast;
}
// Method Definition Index: 117522
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1 (delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, int64_t ___2_ns, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_statement, ___2_ns, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117523
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* delegate_profile_BeginInvoke_m72F814267FC2BD8D58E5882AC63565852606E55D (delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, int64_t ___2_ns, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___3_callback, RuntimeObject* ___4_object, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[4] = {0};
	__d_args[0] = ___0_user_data;
	__d_args[1] = Box(utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25_il2cpp_TypeInfo_var, &___1_statement);
	__d_args[2] = Box(il2cpp_defaults.int64_class, &___2_ns);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___3_callback, (RuntimeObject*)___4_object);
}
// Method Definition Index: 117524
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_profile_EndInvoke_m8E3A859AF8CE0EC4B78C0C767AA2708B81D90D14 (delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
int32_t delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_Multicast(delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	int32_t retVal = 0;
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* currentDelegate = reinterpret_cast<delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710*>(delegatesToInvoke[i]);
		typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, const RuntimeMethod*);
		retVal = ((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_user_data, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
	return retVal;
}
int32_t delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_OpenInst(delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, method);
}
int32_t delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_OpenStatic(delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___method_ptr)(___0_user_data, method);
}
int32_t delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_OpenVirtual(delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return VirtualFuncInvoker0< int32_t >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_user_data);
}
int32_t delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_OpenInterface(delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return InterfaceFuncInvoker0< int32_t >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_user_data);
}
int32_t delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_OpenGenericVirtual(delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericVirtualFuncInvoker0< int32_t >::Invoke(method, ___0_user_data);
}
int32_t delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_OpenGenericInterface(delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_user_data);
	return GenericInterfaceFuncInvoker0< int32_t >::Invoke(method, ___0_user_data);
}
// Method Definition Index: 117525
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_progress__ctor_mA69095A6759BF1B526140E8B225E067852C264CE (delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 1;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 0;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_Multicast;
}
// Method Definition Index: 117526
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218 (delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117527
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* delegate_progress_BeginInvoke_m60807C419C874045368697C5C5E6BBE12364969C (delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* __this, RuntimeObject* ___0_user_data, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___1_callback, RuntimeObject* ___2_object, const RuntimeMethod* method) 
{
	void *__d_args[2] = {0};
	__d_args[0] = ___0_user_data;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___1_callback, (RuntimeObject*)___2_object);
}
// Method Definition Index: 117528
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t delegate_progress_EndInvoke_m48B7597D9A19E3DD885AE799116649FF0FECCD80 (delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
	return *(int32_t*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
void delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_Multicast(delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* currentDelegate = reinterpret_cast<delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5*>(delegatesToInvoke[i]);
		typedef void (*FunctionPointerType) (RuntimeObject*, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894*, const RuntimeMethod*);
		((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_ctx, ___1_user_data, ___2_args, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
}
void delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_OpenInst(delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	typedef void (*FunctionPointerType) (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894*, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_ctx, ___1_user_data, ___2_args, method);
}
void delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_OpenStatic(delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method)
{
	typedef void (*FunctionPointerType) (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894*, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_ctx, ___1_user_data, ___2_args, method);
}
void delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_OpenVirtual(delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	VirtualActionInvoker2< RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_ctx, ___1_user_data, ___2_args);
}
void delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_OpenInterface(delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	InterfaceActionInvoker2< RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_ctx, ___1_user_data, ___2_args);
}
void delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_OpenGenericVirtual(delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	GenericVirtualActionInvoker2< RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* >::Invoke(method, ___0_ctx, ___1_user_data, ___2_args);
}
void delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_OpenGenericInterface(delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	GenericInterfaceActionInvoker2< RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* >::Invoke(method, ___0_ctx, ___1_user_data, ___2_args);
}
// Method Definition Index: 117529
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_function_scalar__ctor_mD850A6C8F1D8268869399E131CC060075101EF31 (delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 3;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 2;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_Multicast;
}
// Method Definition Index: 117530
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D (delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894*, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_ctx, ___1_user_data, ___2_args, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117531
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* delegate_function_scalar_BeginInvoke_m0C25CE6D9ADB34AA9A136E730F37D38ADF1F2B76 (delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___3_callback, RuntimeObject* ___4_object, const RuntimeMethod* method) 
{
	void *__d_args[4] = {0};
	__d_args[0] = ___0_ctx;
	__d_args[1] = ___1_user_data;
	__d_args[2] = ___2_args;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___3_callback, (RuntimeObject*)___4_object);
}
// Method Definition Index: 117532
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_function_scalar_EndInvoke_m96BCBFBB72E4E90C22A060CC8478AC37788C0EA8 (delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
void delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_Multicast(delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* currentDelegate = reinterpret_cast<delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957*>(delegatesToInvoke[i]);
		typedef void (*FunctionPointerType) (RuntimeObject*, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894*, const RuntimeMethod*);
		((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_ctx, ___1_user_data, ___2_args, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
}
void delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_OpenInst(delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	typedef void (*FunctionPointerType) (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894*, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_ctx, ___1_user_data, ___2_args, method);
}
void delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_OpenStatic(delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method)
{
	typedef void (*FunctionPointerType) (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894*, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_ctx, ___1_user_data, ___2_args, method);
}
void delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_OpenVirtual(delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	VirtualActionInvoker2< RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_ctx, ___1_user_data, ___2_args);
}
void delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_OpenInterface(delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	InterfaceActionInvoker2< RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_ctx, ___1_user_data, ___2_args);
}
void delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_OpenGenericVirtual(delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	GenericVirtualActionInvoker2< RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* >::Invoke(method, ___0_ctx, ___1_user_data, ___2_args);
}
void delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_OpenGenericInterface(delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	GenericInterfaceActionInvoker2< RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* >::Invoke(method, ___0_ctx, ___1_user_data, ___2_args);
}
// Method Definition Index: 117533
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_function_aggregate_step__ctor_m961A94C8BFC3C6118100DE85B4EE352BA3FAE76A (delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 3;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 2;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_Multicast;
}
// Method Definition Index: 117534
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511 (delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894*, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_ctx, ___1_user_data, ___2_args, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117535
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* delegate_function_aggregate_step_BeginInvoke_m491F3A03230B1C9AC6D0DFC2C8BD953F47FDCC37 (delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___3_callback, RuntimeObject* ___4_object, const RuntimeMethod* method) 
{
	void *__d_args[4] = {0};
	__d_args[0] = ___0_ctx;
	__d_args[1] = ___1_user_data;
	__d_args[2] = ___2_args;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___3_callback, (RuntimeObject*)___4_object);
}
// Method Definition Index: 117536
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_function_aggregate_step_EndInvoke_m5B7ABB8E94AB970CFA5C801FF1C427421F670613 (delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
void delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_Multicast(delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, const RuntimeMethod* method)
{
	il2cpp_array_size_t length = __this->___delegates->max_length;
	Delegate_t** delegatesToInvoke = reinterpret_cast<Delegate_t**>(__this->___delegates->GetAddressAtUnchecked(0));
	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* currentDelegate = reinterpret_cast<delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0*>(delegatesToInvoke[i]);
		typedef void (*FunctionPointerType) (RuntimeObject*, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, const RuntimeMethod*);
		((FunctionPointerType)currentDelegate->___invoke_impl)((Il2CppObject*)currentDelegate->___method_code, ___0_ctx, ___1_user_data, reinterpret_cast<RuntimeMethod*>(currentDelegate->___method));
	}
}
void delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_OpenInst(delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	typedef void (*FunctionPointerType) (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_ctx, ___1_user_data, method);
}
void delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_OpenStatic(delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, const RuntimeMethod* method)
{
	typedef void (*FunctionPointerType) (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, const RuntimeMethod*);
	((FunctionPointerType)__this->___method_ptr)(___0_ctx, ___1_user_data, method);
}
void delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_OpenVirtual(delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	VirtualActionInvoker1< RuntimeObject* >::Invoke(il2cpp_codegen_method_get_slot(method), ___0_ctx, ___1_user_data);
}
void delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_OpenInterface(delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	InterfaceActionInvoker1< RuntimeObject* >::Invoke(il2cpp_codegen_method_get_slot(method), il2cpp_codegen_method_get_declaring_type(method), ___0_ctx, ___1_user_data);
}
void delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_OpenGenericVirtual(delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	GenericVirtualActionInvoker1< RuntimeObject* >::Invoke(method, ___0_ctx, ___1_user_data);
}
void delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_OpenGenericInterface(delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, const RuntimeMethod* method)
{
	NullCheck(___0_ctx);
	GenericInterfaceActionInvoker1< RuntimeObject* >::Invoke(method, ___0_ctx, ___1_user_data);
}
// Method Definition Index: 117537
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_function_aggregate_final__ctor_m8047213417D49A316609529AE64FEDD0A5BC4019 (delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* __this, RuntimeObject* ___0_object, intptr_t ___1_method, const RuntimeMethod* method) 
{
	__this->___method_ptr = (intptr_t)il2cpp_codegen_get_method_pointer((RuntimeMethod*)___1_method);
	__this->___method = ___1_method;
	__this->___m_target = ___0_object;
	Il2CppCodeGenWriteBarrier((void**)(&__this->___m_target), (void*)___0_object);
	int parameterCount = il2cpp_codegen_method_parameter_count((RuntimeMethod*)___1_method);
	__this->___method_code = (intptr_t)__this;
	if (MethodIsStatic((RuntimeMethod*)___1_method))
	{
		bool isOpen = parameterCount == 2;
		if (isOpen)
			__this->___invoke_impl = (intptr_t)&delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_OpenStatic;
		else
			{
				__this->___invoke_impl = __this->___method_ptr;
				__this->___method_code = (intptr_t)__this->___m_target;
			}
	}
	else
	{
		bool isOpen = parameterCount == 1;
		if (isOpen)
		{
			if (__this->___method_is_virtual)
			{
				if (il2cpp_codegen_method_is_generic_instance_method((RuntimeMethod*)___1_method))
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_OpenGenericInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_OpenGenericVirtual;
				else
					if (il2cpp_codegen_method_is_interface_method((RuntimeMethod*)___1_method))
						__this->___invoke_impl = (intptr_t)&delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_OpenInterface;
					else
						__this->___invoke_impl = (intptr_t)&delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_OpenVirtual;
			}
			else
			{
				__this->___invoke_impl = (intptr_t)&delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_OpenInst;
			}
		}
		else
		{
			if (___0_object == NULL)
				il2cpp_codegen_raise_exception(il2cpp_codegen_get_argument_exception(NULL, "Delegate to an instance method cannot have null 'this'."), NULL);
			__this->___invoke_impl = __this->___method_ptr;
			__this->___method_code = (intptr_t)__this->___m_target;
		}
	}
	__this->___extra_arg = (intptr_t)&delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_Multicast;
}
// Method Definition Index: 117538
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98 (delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_ctx, ___1_user_data, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117539
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* delegate_function_aggregate_final_BeginInvoke_m0B963AC707DC5BEC7373BF80822FE122603539E8 (delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, AsyncCallback_t7FEF460CBDCFB9C5FA2EF776984778B9A4145F4C* ___2_callback, RuntimeObject* ___3_object, const RuntimeMethod* method) 
{
	void *__d_args[3] = {0};
	__d_args[0] = ___0_ctx;
	__d_args[1] = ___1_user_data;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___2_callback, (RuntimeObject*)___3_object);
}
// Method Definition Index: 117540
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void delegate_function_aggregate_final_EndInvoke_mD5A899DCBD2315E8A137D270ECA2C3512A75C9BC (delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* __this, RuntimeObject* ___0_result, const RuntimeMethod* method) 
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___0_result, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117692
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 util_to_utf8z_m1DF23302D4137D1FBBC9512E43A295235A5F0B48 (String_t* ___0_s, const RuntimeMethod* method) 
{
	{
		String_t* L_0 = ___0_s;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_1;
		L_1 = utf8z_FromString_m7BB9CCF1090502FE22763B511942841646A49A2D(L_0, NULL);
		return L_1;
	}
}
// Method Definition Index: 117693
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* util_to_utf8_with_z_m3C8DD1FCA689A8E3D7C8AD391BAAFCAC45F6596E (String_t* ___0_sourceText, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* V_0 = NULL;
	int32_t V_1 = 0;
	{
		String_t* L_0 = ___0_sourceText;
		if (L_0)
		{
			goto IL_0005;
		}
	}
	{
		return (ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)NULL;
	}

IL_0005:
	{
		Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* L_1;
		L_1 = Encoding_get_UTF8_m9FA98A53CE96FD6D02982625C5246DD36C1235C9(NULL);
		String_t* L_2 = ___0_sourceText;
		NullCheck(L_1);
		int32_t L_3;
		L_3 = VirtualFuncInvoker1< int32_t, String_t* >::Invoke(11, L_1, L_2);
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_4 = (ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)(ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*)SZArrayNew(ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031_il2cpp_TypeInfo_var, (uint32_t)((int32_t)il2cpp_codegen_add(L_3, 1)));
		V_0 = L_4;
		Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* L_5;
		L_5 = Encoding_get_UTF8_m9FA98A53CE96FD6D02982625C5246DD36C1235C9(NULL);
		String_t* L_6 = ___0_sourceText;
		String_t* L_7 = ___0_sourceText;
		NullCheck(L_7);
		int32_t L_8;
		L_8 = String_get_Length_m42625D67623FA5CC7A44D47425CE86FB946542D2_inline(L_7, NULL);
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_9 = V_0;
		NullCheck(L_5);
		int32_t L_10;
		L_10 = VirtualFuncInvoker5< int32_t, String_t*, int32_t, int32_t, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*, int32_t >::Invoke(19, L_5, L_6, 0, L_8, L_9, 0);
		V_1 = L_10;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_11 = V_0;
		int32_t L_12 = V_1;
		NullCheck(L_11);
		(L_11)->SetAt(static_cast<il2cpp_array_size_t>(L_12), (uint8_t)0);
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_13 = V_0;
		return L_13;
	}
}
// Method Definition Index: 117694
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t util_my_strlen_m19F785A27156B83F63B0351BEEA1D63D47D10E61 (intptr_t ___0_nativeString, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	{
		V_0 = 0;
		intptr_t L_0 = ___0_nativeString;
		bool L_1;
		L_1 = IntPtr_op_Inequality_m90EFC9C4CAD9A33E309F2DDF98EE4E1DD253637B_inline(L_0, 0, NULL);
		if (!L_1)
		{
			goto IL_001f;
		}
	}
	{
		goto IL_0015;
	}

IL_0011:
	{
		int32_t L_2 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add(L_2, 1));
	}

IL_0015:
	{
		intptr_t L_3 = ___0_nativeString;
		int32_t L_4 = V_0;
		il2cpp_codegen_runtime_class_init_inline(Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		uint8_t L_5;
		L_5 = Marshal_ReadByte_m40222A943AEA82FBFAC5D4881CABD56DFFBA7085(L_3, L_4, NULL);
		if ((((int32_t)L_5) > ((int32_t)0)))
		{
			goto IL_0011;
		}
	}

IL_001f:
	{
		int32_t L_6 = V_0;
		return L_6;
	}
}
// Method Definition Index: 117695
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* util_from_utf8_z_mA4AD3FF9FAB5CA653DD808B6B17A04FDF7425743 (intptr_t ___0_nativeString, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ___0_nativeString;
		intptr_t L_1 = ___0_nativeString;
		int32_t L_2;
		L_2 = util_my_strlen_m19F785A27156B83F63B0351BEEA1D63D47D10E61(L_1, NULL);
		String_t* L_3;
		L_3 = util_from_utf8_mD7425F48F00DC0C3573A6E3C53744D6C114C52B6(L_0, L_2, NULL);
		return L_3;
	}
}
// Method Definition Index: 117696
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* util_from_utf8_mD7425F48F00DC0C3573A6E3C53744D6C114C52B6 (intptr_t ___0_nativeString, int32_t ___1_size, const RuntimeMethod* method) 
{
	String_t* V_0 = NULL;
	{
		V_0 = (String_t*)NULL;
		intptr_t L_0 = ___0_nativeString;
		bool L_1;
		L_1 = IntPtr_op_Inequality_m90EFC9C4CAD9A33E309F2DDF98EE4E1DD253637B_inline(L_0, 0, NULL);
		if (!L_1)
		{
			goto IL_0022;
		}
	}
	{
		Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* L_2;
		L_2 = Encoding_get_UTF8_m9FA98A53CE96FD6D02982625C5246DD36C1235C9(NULL);
		void* L_3;
		L_3 = IntPtr_ToPointer_m1A0612EED3A1C8B8850BE2943CFC42523064B4F6_inline((&___0_nativeString), NULL);
		int32_t L_4 = ___1_size;
		NullCheck(L_2);
		String_t* L_5;
		L_5 = Encoding_GetString_m42BFF0862341DCD5289A7D75B5D7A22CE9690EAD(L_2, (uint8_t*)L_3, L_4, NULL);
		V_0 = L_5;
	}

IL_0022:
	{
		String_t* L_6 = V_0;
		return L_6;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117697
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint8_t* utf8z_GetPinnableReference_m7FC3FFCB77E49E28512035FDEF8CF181E2D39FE5 (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_GetPinnableReference_m365106BC7655B6A45D47673D473A699B5B69DA57_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* L_0 = (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D*)(&__this->___sp);
		uint8_t* L_1;
		L_1 = ReadOnlySpan_1_GetPinnableReference_m365106BC7655B6A45D47673D473A699B5B69DA57(L_0, ReadOnlySpan_1_GetPinnableReference_m365106BC7655B6A45D47673D473A699B5B69DA57_RuntimeMethod_var);
		return L_1;
	}
}
IL2CPP_EXTERN_C  uint8_t* utf8z_GetPinnableReference_m7FC3FFCB77E49E28512035FDEF8CF181E2D39FE5_AdjustorThunk (RuntimeObject* __this, const RuntimeMethod* method)
{
	utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25*>(__this + _offset);
	uint8_t* _returnValue;
	_returnValue = utf8z_GetPinnableReference_m7FC3FFCB77E49E28512035FDEF8CF181E2D39FE5(_thisAdjusted, method);
	return _returnValue;
}
// Method Definition Index: 117698
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void utf8z__ctor_m4B9DBE66D43FBDB2E4AF2D6DAADBD85A87366FFD (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* __this, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___0_a, const RuntimeMethod* method) 
{
	{
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_0 = ___0_a;
		__this->___sp = L_0;
		return;
	}
}
IL2CPP_EXTERN_C  void utf8z__ctor_m4B9DBE66D43FBDB2E4AF2D6DAADBD85A87366FFD_AdjustorThunk (RuntimeObject* __this, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___0_a, const RuntimeMethod* method)
{
	utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25*>(__this + _offset);
	utf8z__ctor_m4B9DBE66D43FBDB2E4AF2D6DAADBD85A87366FFD_inline(_thisAdjusted, ___0_a, method);
}
// Method Definition Index: 117699
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 utf8z_FromSpan_mCA9D0C027632A3293CD3FBB43AF9887C124D1E44 (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___0_span, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		int32_t L_0;
		L_0 = ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_inline((&___0_span), ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		if ((((int32_t)L_0) <= ((int32_t)0)))
		{
			goto IL_0028;
		}
	}
	{
		int32_t L_1;
		L_1 = ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_inline((&___0_span), ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		uint8_t* L_2;
		L_2 = il2cpp_span_get_item((uint8_t*)((Il2CppByReference*)&(((&___0_span))->____pointer))->value, (((int32_t)il2cpp_codegen_subtract(L_1, 1))), ((&___0_span))->____length);
		int32_t L_3 = *((uint8_t*)L_2);
		if (!L_3)
		{
			goto IL_0028;
		}
	}
	{
		ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263* L_4 = (ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&ArgumentException_tAD90411542A20A9C72D5CDA3A84181D8B947A263_il2cpp_TypeInfo_var)));
		ArgumentException__ctor_m026938A67AF9D36BB7ED27F80425D7194B514465(L_4, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral5F8F7F20A301184F38050D40710A390698DAEC1D)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_4, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&utf8z_FromSpan_mCA9D0C027632A3293CD3FBB43AF9887C124D1E44_RuntimeMethod_var)));
	}

IL_0028:
	{
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_5 = ___0_span;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_6;
		memset((&L_6), 0, sizeof(L_6));
		utf8z__ctor_m4B9DBE66D43FBDB2E4AF2D6DAADBD85A87366FFD_inline((&L_6), L_5, NULL);
		return L_6;
	}
}
// Method Definition Index: 117700
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 utf8z_FromString_m7BB9CCF1090502FE22763B511942841646A49A2D (String_t* ___0_s, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_op_Implicit_mCEA7A54A72D5D6EADEFE280B4927119123C8E644_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		String_t* L_0 = ___0_s;
		if (L_0)
		{
			goto IL_000e;
		}
	}
	{
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_1;
		L_1 = ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27(ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27_RuntimeMethod_var);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		memset((&L_2), 0, sizeof(L_2));
		utf8z__ctor_m4B9DBE66D43FBDB2E4AF2D6DAADBD85A87366FFD_inline((&L_2), L_1, NULL);
		return L_2;
	}

IL_000e:
	{
		String_t* L_3 = ___0_s;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_4;
		L_4 = util_to_utf8_with_z_m3C8DD1FCA689A8E3D7C8AD391BAAFCAC45F6596E(L_3, NULL);
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_5;
		L_5 = ReadOnlySpan_1_op_Implicit_mCEA7A54A72D5D6EADEFE280B4927119123C8E644(L_4, ReadOnlySpan_1_op_Implicit_mCEA7A54A72D5D6EADEFE280B4927119123C8E644_RuntimeMethod_var);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_6;
		memset((&L_6), 0, sizeof(L_6));
		utf8z__ctor_m4B9DBE66D43FBDB2E4AF2D6DAADBD85A87366FFD_inline((&L_6), L_5, NULL);
		return L_6;
	}
}
// Method Definition Index: 117701
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t utf8z_my_strlen_mD6037181019A99D60C76A123A59331DFAA86EE8E (uint8_t* ___0_p, const RuntimeMethod* method) 
{
	uint8_t* V_0 = NULL;
	{
		uint8_t* L_0 = ___0_p;
		V_0 = L_0;
		goto IL_0008;
	}

IL_0004:
	{
		uint8_t* L_1 = V_0;
		V_0 = ((uint8_t*)il2cpp_codegen_add((intptr_t)L_1, 1));
	}

IL_0008:
	{
		uint8_t* L_2 = V_0;
		int32_t L_3 = (*(L_2));
		if (L_3)
		{
			goto IL_0004;
		}
	}
	{
		uint8_t* L_4 = V_0;
		uint8_t* L_5 = ___0_p;
		return ((int64_t)(intptr_t)((uint8_t*)((intptr_t)((uint8_t*)il2cpp_codegen_subtract((intptr_t)L_4, (intptr_t)L_5))/1)));
	}
}
// Method Definition Index: 117702
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D utf8z_find_zero_terminator_m6B5DD136DDAF627919C425944E08F3DC3BC1F7AB (uint8_t* ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1__ctor_m470D1527EF015478E8677C7BCB52C8410A1DB604_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	{
		uint8_t* L_0 = ___0_p;
		int64_t L_1;
		L_1 = utf8z_my_strlen_mD6037181019A99D60C76A123A59331DFAA86EE8E(L_0, NULL);
		V_0 = ((int32_t)L_1);
		uint8_t* L_2 = ___0_p;
		int32_t L_3 = V_0;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_4;
		memset((&L_4), 0, sizeof(L_4));
		ReadOnlySpan_1__ctor_m470D1527EF015478E8677C7BCB52C8410A1DB604_inline((&L_4), (void*)L_2, ((int32_t)il2cpp_codegen_add(L_3, 1)), ReadOnlySpan_1__ctor_m470D1527EF015478E8677C7BCB52C8410A1DB604_RuntimeMethod_var);
		return L_4;
	}
}
// Method Definition Index: 117703
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 utf8z_FromPtr_m79B82D5AA2EB5BF1F8E2F5FBA2A65AB9F8DDA116 (uint8_t* ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		uint8_t* L_0 = ___0_p;
		if ((!(((uintptr_t)L_0) == ((uintptr_t)((uintptr_t)0)))))
		{
			goto IL_0010;
		}
	}
	{
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_1;
		L_1 = ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27(ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27_RuntimeMethod_var);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		memset((&L_2), 0, sizeof(L_2));
		utf8z__ctor_m4B9DBE66D43FBDB2E4AF2D6DAADBD85A87366FFD_inline((&L_2), L_1, NULL);
		return L_2;
	}

IL_0010:
	{
		uint8_t* L_3 = ___0_p;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_4;
		L_4 = utf8z_find_zero_terminator_m6B5DD136DDAF627919C425944E08F3DC3BC1F7AB(L_3, NULL);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_5;
		memset((&L_5), 0, sizeof(L_5));
		utf8z__ctor_m4B9DBE66D43FBDB2E4AF2D6DAADBD85A87366FFD_inline((&L_5), L_4, NULL);
		return L_5;
	}
}
// Method Definition Index: 117704
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 utf8z_FromPtrLen_m1B6B151AAF2F122387D310CB69A3C45948F9F098 (uint8_t* ___0_p, int32_t ___1_len, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1__ctor_m470D1527EF015478E8677C7BCB52C8410A1DB604_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		uint8_t* L_0 = ___0_p;
		if ((!(((uintptr_t)L_0) == ((uintptr_t)((uintptr_t)0)))))
		{
			goto IL_0010;
		}
	}
	{
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_1;
		L_1 = ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27(ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27_RuntimeMethod_var);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2;
		memset((&L_2), 0, sizeof(L_2));
		utf8z__ctor_m4B9DBE66D43FBDB2E4AF2D6DAADBD85A87366FFD_inline((&L_2), L_1, NULL);
		return L_2;
	}

IL_0010:
	{
		uint8_t* L_3 = ___0_p;
		int32_t L_4 = ___1_len;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_5;
		memset((&L_5), 0, sizeof(L_5));
		ReadOnlySpan_1__ctor_m470D1527EF015478E8677C7BCB52C8410A1DB604_inline((&L_5), (void*)L_3, ((int32_t)il2cpp_codegen_add(L_4, 1)), ReadOnlySpan_1__ctor_m470D1527EF015478E8677C7BCB52C8410A1DB604_RuntimeMethod_var);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_6;
		L_6 = utf8z_FromSpan_mCA9D0C027632A3293CD3FBB43AF9887C124D1E44(L_5, NULL);
		return L_6;
	}
}
// Method Definition Index: 117705
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 utf8z_FromIntPtr_m6E92ABD379F45CD90E4085B8886EEA755813EC79 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ___0_p;
		bool L_1;
		L_1 = IntPtr_op_Equality_m7D9CDCDE9DC2A0C2C614633F4921E90187FAB271_inline(L_0, 0, NULL);
		if (!L_1)
		{
			goto IL_0018;
		}
	}
	{
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_2;
		L_2 = ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27(ReadOnlySpan_1_get_Empty_mF590D02EC6334390A18F6F05B80FDD62991A3C27_RuntimeMethod_var);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3;
		memset((&L_3), 0, sizeof(L_3));
		utf8z__ctor_m4B9DBE66D43FBDB2E4AF2D6DAADBD85A87366FFD_inline((&L_3), L_2, NULL);
		return L_3;
	}

IL_0018:
	{
		void* L_4;
		L_4 = IntPtr_ToPointer_m1A0612EED3A1C8B8850BE2943CFC42523064B4F6_inline((&___0_p), NULL);
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_5;
		L_5 = utf8z_find_zero_terminator_m6B5DD136DDAF627919C425944E08F3DC3BC1F7AB((uint8_t*)L_4, NULL);
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_6;
		memset((&L_6), 0, sizeof(L_6));
		utf8z__ctor_m4B9DBE66D43FBDB2E4AF2D6DAADBD85A87366FFD_inline((&L_6), L_5, NULL);
		return L_6;
	}
}
// Method Definition Index: 117706
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_GetPinnableReference_m365106BC7655B6A45D47673D473A699B5B69DA57_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	uint8_t* V_0 = NULL;
	uint8_t* V_1 = NULL;
	{
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* L_0 = (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D*)(&__this->___sp);
		int32_t L_1;
		L_1 = ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_inline(L_0, ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		if (L_1)
		{
			goto IL_000f;
		}
	}
	{
		return (String_t*)NULL;
	}

IL_000f:
	{
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* L_2 = (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D*)(&__this->___sp);
		uint8_t* L_3;
		L_3 = ReadOnlySpan_1_GetPinnableReference_m365106BC7655B6A45D47673D473A699B5B69DA57(L_2, ReadOnlySpan_1_GetPinnableReference_m365106BC7655B6A45D47673D473A699B5B69DA57_RuntimeMethod_var);
		V_1 = L_3;
		uint8_t* L_4 = V_1;
		V_0 = (uint8_t*)((uintptr_t)L_4);
		Encoding_t65CDEF28CF20A7B8C92E85A4E808920C2465F095* L_5;
		L_5 = Encoding_get_UTF8_m9FA98A53CE96FD6D02982625C5246DD36C1235C9(NULL);
		uint8_t* L_6 = V_0;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* L_7 = (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D*)(&__this->___sp);
		int32_t L_8;
		L_8 = ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_inline(L_7, ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_RuntimeMethod_var);
		NullCheck(L_5);
		String_t* L_9;
		L_9 = Encoding_GetString_m42BFF0862341DCD5289A7D75B5D7A22CE9690EAD(L_5, L_6, ((int32_t)il2cpp_codegen_subtract(L_8, 1)), NULL);
		return L_9;
	}
}
IL2CPP_EXTERN_C  String_t* utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A_AdjustorThunk (RuntimeObject* __this, const RuntimeMethod* method)
{
	utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25*>(__this + _offset);
	String_t* _returnValue;
	_returnValue = utf8z_utf8_to_string_m580270DDB4AE9B6A5AAC5C4FA552263D9C29C90A(_thisAdjusted, method);
	return _returnValue;
}
// Method Definition Index: 117707
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* utf8z_GetZeroTerminatedUTF8Bytes_mA00D0575203130397CE52DC6AC4718FBA60D853F (String_t* ___0_value, const RuntimeMethod* method) 
{
	{
		String_t* L_0 = ___0_value;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_1;
		L_1 = util_to_utf8_with_z_m3C8DD1FCA689A8E3D7C8AD391BAAFCAC45F6596E(L_0, NULL);
		return L_1;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117708
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PreserveAttribute__ctor_m2FAE0E78E4C140F75181939282430DB4B0CC1235 (PreserveAttribute_t684A021602CCD4E643B8148B5315F6EDDCCD0D12* __this, const RuntimeMethod* method) 
{
	{
		Attribute__ctor_m79ED1BF1EE36D1E417BA89A0D9F91F8AAD8D19E2(__this, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117709
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MonoPInvokeCallbackAttribute__ctor_mA8958BAFDBE3EDC1FC5ED611D0711FCE982EB87C (MonoPInvokeCallbackAttribute_t17BE665B3D7CD95379096445F10A2E7124970063* __this, Type_t* ___0_t, const RuntimeMethod* method) 
{
	{
		Attribute__ctor_m79ED1BF1EE36D1E417BA89A0D9F91F8AAD8D19E2(__this, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117710
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SafeGCHandle__ctor_mB8C029FD49878D0939689FF0B31B855D9279196E (SafeGCHandle_tE8D8C107E75BFC0FAC19C271622895B407785D3E* __this, RuntimeObject* ___0_v, int32_t ___1_typ, const RuntimeMethod* method) 
{
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		SafeHandle__ctor_m23E44C94503043292DCD4E87818082CFC09A7F4B(__this, 0, (bool)1, NULL);
		RuntimeObject* L_0 = ___0_v;
		if (!L_0)
		{
			goto IL_0023;
		}
	}
	{
		RuntimeObject* L_1 = ___0_v;
		int32_t L_2 = ___1_typ;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_3;
		L_3 = GCHandle_Alloc_m3BFD398427352FC756FFE078F01A504B681352EC(L_1, L_2, NULL);
		V_0 = L_3;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_4 = V_0;
		intptr_t L_5;
		L_5 = GCHandle_ToIntPtr_m45294AA913461A070BD555F81103A8BF2E5ED976(L_4, NULL);
		SafeHandle_SetHandle_m003D64748F9DFBA1E3C0B23798C23BA81AA21C2A_inline(__this, L_5, NULL);
	}

IL_0023:
	{
		return;
	}
}
// Method Definition Index: 117711
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SafeGCHandle_get_IsInvalid_m61EA73C2D6115FC47D488C39D53F52F8EDAD8109 (SafeGCHandle_tE8D8C107E75BFC0FAC19C271622895B407785D3E* __this, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		bool L_1;
		L_1 = IntPtr_op_Equality_m7D9CDCDE9DC2A0C2C614633F4921E90187FAB271_inline(L_0, 0, NULL);
		return L_1;
	}
}
// Method Definition Index: 117712
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SafeGCHandle_ReleaseHandle_m6F483FD3DBA601DA04F6D7C82376844E51B2B56A (SafeGCHandle_tE8D8C107E75BFC0FAC19C271622895B407785D3E* __this, const RuntimeMethod* method) 
{
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_1;
		L_1 = GCHandle_FromIntPtr_mA7848A4285D007CADC52B6272DB243C8FDFD5FAC(L_0, NULL);
		V_0 = L_1;
		GCHandle_Free_m1320A260E487EB1EA6D95F9E54BFFCB5A4EF83A3((&V_0), NULL);
		return (bool)1;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117713
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void hook_handle__ctor_m6D8E37A8ADD1DC2ED8DAB0482D5E7212F4BBC524 (hook_handle_tADC84A43AFFDADC460E97AB35367B400F2EB2EFC* __this, RuntimeObject* ___0_target, const RuntimeMethod* method) 
{
	{
		RuntimeObject* L_0 = ___0_target;
		SafeGCHandle__ctor_mB8C029FD49878D0939689FF0B31B855D9279196E(__this, L_0, 2, NULL);
		return;
	}
}
// Method Definition Index: 117714
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* hook_handle_ForDispose_m75CB5080133A1EA42368D022BC358117AC9FE588 (hook_handle_tADC84A43AFFDADC460E97AB35367B400F2EB2EFC* __this, const RuntimeMethod* method) 
{
	{
		bool L_0;
		L_0 = VirtualFuncInvoker0< bool >::Invoke(5, __this);
		if (!L_0)
		{
			goto IL_000a;
		}
	}
	{
		return (RuntimeObject*)NULL;
	}

IL_000a:
	{
		return __this;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117715
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CompareBuf__ctor_m6C58AEB15ADFE4A32C2EC9DDDA455E5A929F92E0 (CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA* __this, Func_4_t969FE6B8E28BD4E700C6BA2ED5F8794B88E37083* ___0_f, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&EqualityComparer_1__ctor_m98FB31E1349FDCF9F0A3BD1891F322A1AC6CFBC6_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		EqualityComparer_1__ctor_m98FB31E1349FDCF9F0A3BD1891F322A1AC6CFBC6(__this, EqualityComparer_1__ctor_m98FB31E1349FDCF9F0A3BD1891F322A1AC6CFBC6_RuntimeMethod_var);
		Func_4_t969FE6B8E28BD4E700C6BA2ED5F8794B88E37083* L_0 = ___0_f;
		__this->____f = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____f), (void*)L_0);
		return;
	}
}
// Method Definition Index: 117716
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool CompareBuf_Equals_m6E5986CB1C0D646F268039A924B0184019F1FFA6 (CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_p1, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___1_p2, const RuntimeMethod* method) 
{
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_0;
	memset((&V_0), 0, sizeof(V_0));
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_0 = ___0_p1;
		NullCheck(L_0);
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_1 = ___1_p2;
		NullCheck(L_1);
		if ((((int32_t)((int32_t)(((RuntimeArray*)L_0)->max_length))) == ((int32_t)((int32_t)(((RuntimeArray*)L_1)->max_length)))))
		{
			goto IL_000a;
		}
	}
	{
		return (bool)0;
	}

IL_000a:
	{
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_2 = ___0_p1;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_3;
		L_3 = GCHandle_Alloc_m3BFD398427352FC756FFE078F01A504B681352EC((RuntimeObject*)L_2, 3, NULL);
		V_0 = L_3;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_4 = ___1_p2;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_5;
		L_5 = GCHandle_Alloc_m3BFD398427352FC756FFE078F01A504B681352EC((RuntimeObject*)L_4, 3, NULL);
		V_1 = L_5;
		Func_4_t969FE6B8E28BD4E700C6BA2ED5F8794B88E37083* L_6 = __this->____f;
		intptr_t L_7;
		L_7 = GCHandle_AddrOfPinnedObject_m9C047E154D6F0FE66BE003AB99F0B67A2CA953A6((&V_0), NULL);
		intptr_t L_8;
		L_8 = GCHandle_AddrOfPinnedObject_m9C047E154D6F0FE66BE003AB99F0B67A2CA953A6((&V_1), NULL);
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_9 = ___0_p1;
		NullCheck(L_9);
		NullCheck(L_6);
		bool L_10;
		L_10 = Func_4_Invoke_mF686DB15C7046521DFA2350715743471581C6580_inline(L_6, L_7, L_8, ((int32_t)(((RuntimeArray*)L_9)->max_length)), NULL);
		GCHandle_Free_m1320A260E487EB1EA6D95F9E54BFFCB5A4EF83A3((&V_0), NULL);
		GCHandle_Free_m1320A260E487EB1EA6D95F9E54BFFCB5A4EF83A3((&V_1), NULL);
		return L_10;
	}
}
// Method Definition Index: 117717
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t CompareBuf_GetHashCode_mA57EDEA33292F06D9E0833EDE38AC8C90C5A295E (CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_p, const RuntimeMethod* method) 
{
	{
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_0 = ___0_p;
		NullCheck(L_0);
		return ((int32_t)(((RuntimeArray*)L_0)->max_length));
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117718
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* FuncName_get_name_mB74437A157D3DF6F5D2A693DC4B2C5DC3E47D648 (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* __this, const RuntimeMethod* method) 
{
	{
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_0 = __this->___U3CnameU3Ek__BackingField;
		return L_0;
	}
}
// Method Definition Index: 117719
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void FuncName_set_name_m2E558A596C0258BE6EE151C469BCD07A6426A937 (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_value, const RuntimeMethod* method) 
{
	{
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_0 = ___0_value;
		__this->___U3CnameU3Ek__BackingField = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3CnameU3Ek__BackingField), (void*)L_0);
		return;
	}
}
// Method Definition Index: 117720
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t FuncName_get_n_m91D9E17080DD4D1D59BD9879BE61EBCF5C88E0C4 (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* __this, const RuntimeMethod* method) 
{
	{
		int32_t L_0 = __this->___U3CnU3Ek__BackingField;
		return L_0;
	}
}
// Method Definition Index: 117721
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void FuncName_set_n_mBBC1FE0A2863C200708D2CE3BAB22423DA29259E (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* __this, int32_t ___0_value, const RuntimeMethod* method) 
{
	{
		int32_t L_0 = ___0_value;
		__this->___U3CnU3Ek__BackingField = L_0;
		return;
	}
}
// Method Definition Index: 117722
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void FuncName__ctor_m7E7689C95B8E84D185844712A71EAE817A2FF240 (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0__name, int32_t ___1__n, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_0 = ___0__name;
		FuncName_set_name_m2E558A596C0258BE6EE151C469BCD07A6426A937_inline(__this, L_0, NULL);
		int32_t L_1 = ___1__n;
		FuncName_set_n_mBBC1FE0A2863C200708D2CE3BAB22423DA29259E_inline(__this, L_1, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117723
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void CompareFuncName__ctor_m286F43C4A7064F1D2822AFA93BD4950F6158E24B (CompareFuncName_tDD5FD56A7AAA8A632A4B49F594E21250BFB40795* __this, RuntimeObject* ___0_ptrlencmp, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&EqualityComparer_1__ctor_mF420C788DC290244A73738F4BC6F567E4B8B065D_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		EqualityComparer_1__ctor_mF420C788DC290244A73738F4BC6F567E4B8B065D(__this, EqualityComparer_1__ctor_mF420C788DC290244A73738F4BC6F567E4B8B065D_RuntimeMethod_var);
		RuntimeObject* L_0 = ___0_ptrlencmp;
		__this->____ptrlencmp = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____ptrlencmp), (void*)L_0);
		return;
	}
}
// Method Definition Index: 117724
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool CompareFuncName_Equals_m0B3D28818C69EC73D0EB87C04E4AD2E0C0CA6DD0 (CompareFuncName_tDD5FD56A7AAA8A632A4B49F594E21250BFB40795* __this, FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* ___0_p1, FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* ___1_p2, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEqualityComparer_1_tED581F2C423FD1B93E069AFE7AA4483EF32AF8DB_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* L_0 = ___0_p1;
		NullCheck(L_0);
		int32_t L_1;
		L_1 = FuncName_get_n_m91D9E17080DD4D1D59BD9879BE61EBCF5C88E0C4_inline(L_0, NULL);
		FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* L_2 = ___1_p2;
		NullCheck(L_2);
		int32_t L_3;
		L_3 = FuncName_get_n_m91D9E17080DD4D1D59BD9879BE61EBCF5C88E0C4_inline(L_2, NULL);
		if ((((int32_t)L_1) == ((int32_t)L_3)))
		{
			goto IL_0010;
		}
	}
	{
		return (bool)0;
	}

IL_0010:
	{
		RuntimeObject* L_4 = __this->____ptrlencmp;
		FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* L_5 = ___0_p1;
		NullCheck(L_5);
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_6;
		L_6 = FuncName_get_name_mB74437A157D3DF6F5D2A693DC4B2C5DC3E47D648_inline(L_5, NULL);
		FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* L_7 = ___1_p2;
		NullCheck(L_7);
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_8;
		L_8 = FuncName_get_name_mB74437A157D3DF6F5D2A693DC4B2C5DC3E47D648_inline(L_7, NULL);
		NullCheck(L_4);
		bool L_9;
		L_9 = InterfaceFuncInvoker2< bool, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031*, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* >::Invoke(0, IEqualityComparer_1_tED581F2C423FD1B93E069AFE7AA4483EF32AF8DB_il2cpp_TypeInfo_var, L_4, L_6, L_8);
		return L_9;
	}
}
// Method Definition Index: 117725
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t CompareFuncName_GetHashCode_m2E75F2201E06E506BB47A69BA0E6D966E7029A84 (CompareFuncName_tDD5FD56A7AAA8A632A4B49F594E21250BFB40795* __this, FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* ___0_p, const RuntimeMethod* method) 
{
	{
		FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* L_0 = ___0_p;
		NullCheck(L_0);
		int32_t L_1;
		L_1 = FuncName_get_n_m91D9E17080DD4D1D59BD9879BE61EBCF5C88E0C4_inline(L_0, NULL);
		FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* L_2 = ___0_p;
		NullCheck(L_2);
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_3;
		L_3 = FuncName_get_name_mB74437A157D3DF6F5D2A693DC4B2C5DC3E47D648_inline(L_2, NULL);
		NullCheck(L_3);
		return ((int32_t)il2cpp_codegen_add(L_1, ((int32_t)(((RuntimeArray*)L_3)->max_length))));
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117726
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void hook_handles__ctor_m1F7D3F0DEC6B04876796D7A4935396F2CD8DD292 (hook_handles_tC761D4D430F6003B396C0AE3B469BDD80B3D4C33* __this, Func_4_t969FE6B8E28BD4E700C6BA2ED5F8794B88E37083* ___0_f, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&CompareFuncName_tDD5FD56A7AAA8A632A4B49F594E21250BFB40795_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2__ctor_mC0BBBB7D4FBDEA79D4636FBDB872E0AC54E2C776_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2__ctor_mD8C0B49C9BC53C925E6FAB2B37BADF318BAA208B_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA* V_0 = NULL;
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		Func_4_t969FE6B8E28BD4E700C6BA2ED5F8794B88E37083* L_0 = ___0_f;
		CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA* L_1 = (CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA*)il2cpp_codegen_object_new(CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA_il2cpp_TypeInfo_var);
		CompareBuf__ctor_m6C58AEB15ADFE4A32C2EC9DDDA455E5A929F92E0(L_1, L_0, NULL);
		V_0 = L_1;
		CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA* L_2 = V_0;
		ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522* L_3 = (ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522*)il2cpp_codegen_object_new(ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522_il2cpp_TypeInfo_var);
		ConcurrentDictionary_2__ctor_mD8C0B49C9BC53C925E6FAB2B37BADF318BAA208B(L_3, L_2, ConcurrentDictionary_2__ctor_mD8C0B49C9BC53C925E6FAB2B37BADF318BAA208B_RuntimeMethod_var);
		__this->___collation = L_3;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___collation), (void*)L_3);
		CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA* L_4 = V_0;
		CompareFuncName_tDD5FD56A7AAA8A632A4B49F594E21250BFB40795* L_5 = (CompareFuncName_tDD5FD56A7AAA8A632A4B49F594E21250BFB40795*)il2cpp_codegen_object_new(CompareFuncName_tDD5FD56A7AAA8A632A4B49F594E21250BFB40795_il2cpp_TypeInfo_var);
		CompareFuncName__ctor_m286F43C4A7064F1D2822AFA93BD4950F6158E24B(L_5, L_4, NULL);
		ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517* L_6 = (ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517*)il2cpp_codegen_object_new(ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517_il2cpp_TypeInfo_var);
		ConcurrentDictionary_2__ctor_mC0BBBB7D4FBDEA79D4636FBDB872E0AC54E2C776(L_6, L_5, ConcurrentDictionary_2__ctor_mC0BBBB7D4FBDEA79D4636FBDB872E0AC54E2C776_RuntimeMethod_var);
		__this->___scalar = L_6;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___scalar), (void*)L_6);
		CompareBuf_t27F605DCD52963E50AAF359FF0792B5EF7FC04EA* L_7 = V_0;
		CompareFuncName_tDD5FD56A7AAA8A632A4B49F594E21250BFB40795* L_8 = (CompareFuncName_tDD5FD56A7AAA8A632A4B49F594E21250BFB40795*)il2cpp_codegen_object_new(CompareFuncName_tDD5FD56A7AAA8A632A4B49F594E21250BFB40795_il2cpp_TypeInfo_var);
		CompareFuncName__ctor_m286F43C4A7064F1D2822AFA93BD4950F6158E24B(L_8, L_7, NULL);
		ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517* L_9 = (ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517*)il2cpp_codegen_object_new(ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517_il2cpp_TypeInfo_var);
		ConcurrentDictionary_2__ctor_mC0BBBB7D4FBDEA79D4636FBDB872E0AC54E2C776(L_9, L_8, ConcurrentDictionary_2__ctor_mC0BBBB7D4FBDEA79D4636FBDB872E0AC54E2C776_RuntimeMethod_var);
		__this->___agg = L_9;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___agg), (void*)L_9);
		return;
	}
}
// Method Definition Index: 117727
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool hook_handles_RemoveScalarFunction_m9159BE57F6977DCF622FE30506A7F771739B423F (hook_handles_tC761D4D430F6003B396C0AE3B469BDD80B3D4C33* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_name, int32_t ___1_nargs, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2_TryRemove_m1782697D740431D81873A6F2B420EBBDD4FED32D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* V_0 = NULL;
	RuntimeObject* V_1 = NULL;
	{
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_0 = ___0_name;
		int32_t L_1 = ___1_nargs;
		FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* L_2 = (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A*)il2cpp_codegen_object_new(FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A_il2cpp_TypeInfo_var);
		FuncName__ctor_m7E7689C95B8E84D185844712A71EAE817A2FF240(L_2, L_0, L_1, NULL);
		V_0 = L_2;
		ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517* L_3 = __this->___scalar;
		FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* L_4 = V_0;
		NullCheck(L_3);
		bool L_5;
		L_5 = ConcurrentDictionary_2_TryRemove_m1782697D740431D81873A6F2B420EBBDD4FED32D(L_3, L_4, (&V_1), ConcurrentDictionary_2_TryRemove_m1782697D740431D81873A6F2B420EBBDD4FED32D_RuntimeMethod_var);
		if (!L_5)
		{
			goto IL_0020;
		}
	}
	{
		RuntimeObject* L_6 = V_1;
		NullCheck(L_6);
		InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_6);
		return (bool)1;
	}

IL_0020:
	{
		return (bool)0;
	}
}
// Method Definition Index: 117728
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void hook_handles_AddScalarFunction_mD241ED7FC4CFCD555C808E31748100FCAA652047 (hook_handles_tC761D4D430F6003B396C0AE3B469BDD80B3D4C33* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_name, int32_t ___1_nargs, RuntimeObject* ___2_d, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2_set_Item_mB18463C3EF5A1B49665FDDC7A7418F69CA3F69F0_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* V_0 = NULL;
	{
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_0 = ___0_name;
		int32_t L_1 = ___1_nargs;
		FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* L_2 = (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A*)il2cpp_codegen_object_new(FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A_il2cpp_TypeInfo_var);
		FuncName__ctor_m7E7689C95B8E84D185844712A71EAE817A2FF240(L_2, L_0, L_1, NULL);
		V_0 = L_2;
		ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517* L_3 = __this->___scalar;
		FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* L_4 = V_0;
		RuntimeObject* L_5 = ___2_d;
		NullCheck(L_3);
		ConcurrentDictionary_2_set_Item_mB18463C3EF5A1B49665FDDC7A7418F69CA3F69F0(L_3, L_4, L_5, ConcurrentDictionary_2_set_Item_mB18463C3EF5A1B49665FDDC7A7418F69CA3F69F0_RuntimeMethod_var);
		return;
	}
}
// Method Definition Index: 117729
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool hook_handles_RemoveAggFunction_mA42A9716C0A6E544449532021405FA05652BE8C0 (hook_handles_tC761D4D430F6003B396C0AE3B469BDD80B3D4C33* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_name, int32_t ___1_nargs, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2_TryRemove_m1782697D740431D81873A6F2B420EBBDD4FED32D_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* V_0 = NULL;
	RuntimeObject* V_1 = NULL;
	{
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_0 = ___0_name;
		int32_t L_1 = ___1_nargs;
		FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* L_2 = (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A*)il2cpp_codegen_object_new(FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A_il2cpp_TypeInfo_var);
		FuncName__ctor_m7E7689C95B8E84D185844712A71EAE817A2FF240(L_2, L_0, L_1, NULL);
		V_0 = L_2;
		ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517* L_3 = __this->___agg;
		FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* L_4 = V_0;
		NullCheck(L_3);
		bool L_5;
		L_5 = ConcurrentDictionary_2_TryRemove_m1782697D740431D81873A6F2B420EBBDD4FED32D(L_3, L_4, (&V_1), ConcurrentDictionary_2_TryRemove_m1782697D740431D81873A6F2B420EBBDD4FED32D_RuntimeMethod_var);
		if (!L_5)
		{
			goto IL_0020;
		}
	}
	{
		RuntimeObject* L_6 = V_1;
		NullCheck(L_6);
		InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_6);
		return (bool)1;
	}

IL_0020:
	{
		return (bool)0;
	}
}
// Method Definition Index: 117730
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void hook_handles_AddAggFunction_m81386278C3F4916C5052FD699F25319A978F651A (hook_handles_tC761D4D430F6003B396C0AE3B469BDD80B3D4C33* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_name, int32_t ___1_nargs, RuntimeObject* ___2_d, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2_set_Item_mB18463C3EF5A1B49665FDDC7A7418F69CA3F69F0_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* V_0 = NULL;
	{
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_0 = ___0_name;
		int32_t L_1 = ___1_nargs;
		FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* L_2 = (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A*)il2cpp_codegen_object_new(FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A_il2cpp_TypeInfo_var);
		FuncName__ctor_m7E7689C95B8E84D185844712A71EAE817A2FF240(L_2, L_0, L_1, NULL);
		V_0 = L_2;
		ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517* L_3 = __this->___agg;
		FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* L_4 = V_0;
		RuntimeObject* L_5 = ___2_d;
		NullCheck(L_3);
		ConcurrentDictionary_2_set_Item_mB18463C3EF5A1B49665FDDC7A7418F69CA3F69F0(L_3, L_4, L_5, ConcurrentDictionary_2_set_Item_mB18463C3EF5A1B49665FDDC7A7418F69CA3F69F0_RuntimeMethod_var);
		return;
	}
}
// Method Definition Index: 117731
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool hook_handles_RemoveCollation_mAC3B05DE2CB2A4322F0A035E577FCF6194665012 (hook_handles_tC761D4D430F6003B396C0AE3B469BDD80B3D4C33* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_name, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2_TryRemove_m8220F91E42BAB284E34184616737AAB2A4C9FEF6_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* V_0 = NULL;
	{
		ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522* L_0 = __this->___collation;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_1 = ___0_name;
		NullCheck(L_0);
		bool L_2;
		L_2 = ConcurrentDictionary_2_TryRemove_m8220F91E42BAB284E34184616737AAB2A4C9FEF6(L_0, L_1, (&V_0), ConcurrentDictionary_2_TryRemove_m8220F91E42BAB284E34184616737AAB2A4C9FEF6_RuntimeMethod_var);
		if (!L_2)
		{
			goto IL_0018;
		}
	}
	{
		RuntimeObject* L_3 = V_0;
		NullCheck(L_3);
		InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_3);
		return (bool)1;
	}

IL_0018:
	{
		return (bool)0;
	}
}
// Method Definition Index: 117732
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void hook_handles_AddCollation_mFFF6F01B318D1CDF9E0980812744B1251C09343F (hook_handles_tC761D4D430F6003B396C0AE3B469BDD80B3D4C33* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_name, RuntimeObject* ___1_d, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2_set_Item_m0A8DC29085CC370BCE112BE5175D0766C86B5121_RuntimeMethod_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522* L_0 = __this->___collation;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_1 = ___0_name;
		RuntimeObject* L_2 = ___1_d;
		NullCheck(L_0);
		ConcurrentDictionary_2_set_Item_m0A8DC29085CC370BCE112BE5175D0766C86B5121(L_0, L_1, L_2, ConcurrentDictionary_2_set_Item_m0A8DC29085CC370BCE112BE5175D0766C86B5121_RuntimeMethod_var);
		return;
	}
}
// Method Definition Index: 117733
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void hook_handles_Dispose_m026D881EDB0A3F5F14FDE3DF147A1E4C13AF0363 (hook_handles_tC761D4D430F6003B396C0AE3B469BDD80B3D4C33* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2_get_Values_m2C4385FFCDA16FF0FB436BE7E6127E2F473040FC_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&ConcurrentDictionary_2_get_Values_mF2008A5617C18F3F234996E8F3070D5C9F7ABC10_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEnumerable_1_tB6F68D35F9622A77D895A483A02F4DC2907BBAEB_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEnumerator_1_tAA6EEADDF0E7FE9E72FAC727374287C9627ADF3A_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* V_0 = NULL;
	{
		ConcurrentDictionary_2_tEF1D1D45D3DB2E2FEE67A0CE6A8C5E24E3D1D522* L_0 = __this->___collation;
		NullCheck(L_0);
		RuntimeObject* L_1;
		L_1 = ConcurrentDictionary_2_get_Values_m2C4385FFCDA16FF0FB436BE7E6127E2F473040FC(L_0, ConcurrentDictionary_2_get_Values_m2C4385FFCDA16FF0FB436BE7E6127E2F473040FC_RuntimeMethod_var);
		NullCheck(L_1);
		RuntimeObject* L_2;
		L_2 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0, IEnumerable_1_tB6F68D35F9622A77D895A483A02F4DC2907BBAEB_il2cpp_TypeInfo_var, L_1);
		V_0 = L_2;
	}
	{
		auto __finallyBlock = il2cpp::utils::Finally([&]
		{

FINALLY_0028:
			{
				{
					RuntimeObject* L_3 = V_0;
					if (!L_3)
					{
						goto IL_0031;
					}
				}
				{
					RuntimeObject* L_4 = V_0;
					NullCheck(L_4);
					InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_4);
				}

IL_0031:
				{
					return;
				}
			}
		});
		try
		{
			{
				goto IL_001e_1;
			}

IL_0013_1:
			{
				RuntimeObject* L_5 = V_0;
				NullCheck(L_5);
				RuntimeObject* L_6;
				L_6 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0, IEnumerator_1_tAA6EEADDF0E7FE9E72FAC727374287C9627ADF3A_il2cpp_TypeInfo_var, L_5);
				NullCheck(L_6);
				InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_6);
			}

IL_001e_1:
			{
				RuntimeObject* L_7 = V_0;
				NullCheck(L_7);
				bool L_8;
				L_8 = InterfaceFuncInvoker0< bool >::Invoke(0, IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var, L_7);
				if (L_8)
				{
					goto IL_0013_1;
				}
			}
			{
				goto IL_0032;
			}
		}
		catch(Il2CppExceptionWrapper& e)
		{
			__finallyBlock.StoreException(e.ex);
		}
	}

IL_0032:
	{
		ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517* L_9 = __this->___scalar;
		NullCheck(L_9);
		RuntimeObject* L_10;
		L_10 = ConcurrentDictionary_2_get_Values_mF2008A5617C18F3F234996E8F3070D5C9F7ABC10(L_9, ConcurrentDictionary_2_get_Values_mF2008A5617C18F3F234996E8F3070D5C9F7ABC10_RuntimeMethod_var);
		NullCheck(L_10);
		RuntimeObject* L_11;
		L_11 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0, IEnumerable_1_tB6F68D35F9622A77D895A483A02F4DC2907BBAEB_il2cpp_TypeInfo_var, L_10);
		V_0 = L_11;
	}
	{
		auto __finallyBlock = il2cpp::utils::Finally([&]
		{

FINALLY_005a:
			{
				{
					RuntimeObject* L_12 = V_0;
					if (!L_12)
					{
						goto IL_0063;
					}
				}
				{
					RuntimeObject* L_13 = V_0;
					NullCheck(L_13);
					InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_13);
				}

IL_0063:
				{
					return;
				}
			}
		});
		try
		{
			{
				goto IL_0050_1;
			}

IL_0045_1:
			{
				RuntimeObject* L_14 = V_0;
				NullCheck(L_14);
				RuntimeObject* L_15;
				L_15 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0, IEnumerator_1_tAA6EEADDF0E7FE9E72FAC727374287C9627ADF3A_il2cpp_TypeInfo_var, L_14);
				NullCheck(L_15);
				InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_15);
			}

IL_0050_1:
			{
				RuntimeObject* L_16 = V_0;
				NullCheck(L_16);
				bool L_17;
				L_17 = InterfaceFuncInvoker0< bool >::Invoke(0, IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var, L_16);
				if (L_17)
				{
					goto IL_0045_1;
				}
			}
			{
				goto IL_0064;
			}
		}
		catch(Il2CppExceptionWrapper& e)
		{
			__finallyBlock.StoreException(e.ex);
		}
	}

IL_0064:
	{
		ConcurrentDictionary_2_tF47EFE7CCFE1E29580989F631F638E0434111517* L_18 = __this->___agg;
		NullCheck(L_18);
		RuntimeObject* L_19;
		L_19 = ConcurrentDictionary_2_get_Values_mF2008A5617C18F3F234996E8F3070D5C9F7ABC10(L_18, ConcurrentDictionary_2_get_Values_mF2008A5617C18F3F234996E8F3070D5C9F7ABC10_RuntimeMethod_var);
		NullCheck(L_19);
		RuntimeObject* L_20;
		L_20 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0, IEnumerable_1_tB6F68D35F9622A77D895A483A02F4DC2907BBAEB_il2cpp_TypeInfo_var, L_19);
		V_0 = L_20;
	}
	{
		auto __finallyBlock = il2cpp::utils::Finally([&]
		{

FINALLY_008c:
			{
				{
					RuntimeObject* L_21 = V_0;
					if (!L_21)
					{
						goto IL_0095;
					}
				}
				{
					RuntimeObject* L_22 = V_0;
					NullCheck(L_22);
					InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_22);
				}

IL_0095:
				{
					return;
				}
			}
		});
		try
		{
			{
				goto IL_0082_1;
			}

IL_0077_1:
			{
				RuntimeObject* L_23 = V_0;
				NullCheck(L_23);
				RuntimeObject* L_24;
				L_24 = InterfaceFuncInvoker0< RuntimeObject* >::Invoke(0, IEnumerator_1_tAA6EEADDF0E7FE9E72FAC727374287C9627ADF3A_il2cpp_TypeInfo_var, L_23);
				NullCheck(L_24);
				InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_24);
			}

IL_0082_1:
			{
				RuntimeObject* L_25 = V_0;
				NullCheck(L_25);
				bool L_26;
				L_26 = InterfaceFuncInvoker0< bool >::Invoke(0, IEnumerator_t7B609C2FFA6EB5167D9C62A0C32A21DE2F666DAA_il2cpp_TypeInfo_var, L_25);
				if (L_26)
				{
					goto IL_0077_1;
				}
			}
			{
				goto IL_0096;
			}
		}
		catch(Il2CppExceptionWrapper& e)
		{
			__finallyBlock.StoreException(e.ex);
		}
	}

IL_0096:
	{
		RuntimeObject* L_27 = __this->___update;
		if (!L_27)
		{
			goto IL_00a9;
		}
	}
	{
		RuntimeObject* L_28 = __this->___update;
		NullCheck(L_28);
		InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_28);
	}

IL_00a9:
	{
		RuntimeObject* L_29 = __this->___rollback;
		if (!L_29)
		{
			goto IL_00bc;
		}
	}
	{
		RuntimeObject* L_30 = __this->___rollback;
		NullCheck(L_30);
		InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_30);
	}

IL_00bc:
	{
		RuntimeObject* L_31 = __this->___commit;
		if (!L_31)
		{
			goto IL_00cf;
		}
	}
	{
		RuntimeObject* L_32 = __this->___commit;
		NullCheck(L_32);
		InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_32);
	}

IL_00cf:
	{
		RuntimeObject* L_33 = __this->___trace;
		if (!L_33)
		{
			goto IL_00e2;
		}
	}
	{
		RuntimeObject* L_34 = __this->___trace;
		NullCheck(L_34);
		InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_34);
	}

IL_00e2:
	{
		RuntimeObject* L_35 = __this->___profile;
		if (!L_35)
		{
			goto IL_00f5;
		}
	}
	{
		RuntimeObject* L_36 = __this->___profile;
		NullCheck(L_36);
		InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_36);
	}

IL_00f5:
	{
		RuntimeObject* L_37 = __this->___progress;
		if (!L_37)
		{
			goto IL_0108;
		}
	}
	{
		RuntimeObject* L_38 = __this->___progress;
		NullCheck(L_38);
		InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_38);
	}

IL_0108:
	{
		RuntimeObject* L_39 = __this->___authorizer;
		if (!L_39)
		{
			goto IL_011b;
		}
	}
	{
		RuntimeObject* L_40 = __this->___authorizer;
		NullCheck(L_40);
		InterfaceActionInvoker0::Invoke(0, IDisposable_t030E0496B4E0E4E4F086825007979AF51F7248C5_il2cpp_TypeInfo_var, L_40);
	}

IL_011b:
	{
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117734
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void log_hook_info__ctor_m72FEA14AC551F530BD79A2442BF002D7237EFDFC (log_hook_info_tDC91F629B2B139A944265087648F73E1BB31A116* __this, delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* ___0_func, RuntimeObject* ___1_v, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* L_0 = ___0_func;
		__this->____func = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____func), (void*)L_0);
		RuntimeObject* L_1 = ___1_v;
		__this->____user_data = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____user_data), (void*)L_1);
		return;
	}
}
// Method Definition Index: 117735
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR log_hook_info_tDC91F629B2B139A944265087648F73E1BB31A116* log_hook_info_from_ptr_m4ACDB4DE324881C384D383EE3CA41933E31C3A03 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&log_hook_info_tDC91F629B2B139A944265087648F73E1BB31A116_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___0_p;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_1;
		L_1 = GCHandle_op_Explicit_mA5F28206637454AD677BE13DF86C6152190B6F0F(L_0, NULL);
		V_0 = L_1;
		RuntimeObject* L_2;
		L_2 = GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_inline((&V_0), NULL);
		return ((log_hook_info_tDC91F629B2B139A944265087648F73E1BB31A116*)IsInstClass((RuntimeObject*)L_2, log_hook_info_tDC91F629B2B139A944265087648F73E1BB31A116_il2cpp_TypeInfo_var));
	}
}
// Method Definition Index: 117736
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void log_hook_info_call_mBBD1CECB059D785233952F83C5582786DBEFE694 (log_hook_info_tDC91F629B2B139A944265087648F73E1BB31A116* __this, int32_t ___0_rc, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_msg, const RuntimeMethod* method) 
{
	{
		delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* L_0 = __this->____func;
		RuntimeObject* L_1 = __this->____user_data;
		int32_t L_2 = ___0_rc;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3 = ___1_msg;
		NullCheck(L_0);
		delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_inline(L_0, L_1, L_2, L_3, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117737
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* commit_hook_info_get__func_m05BEEED3611D2441FE7BB1D3FA2DC7B53F14B46D (commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* __this, const RuntimeMethod* method) 
{
	{
		delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* L_0 = __this->___U3C_funcU3Ek__BackingField;
		return L_0;
	}
}
// Method Definition Index: 117738
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void commit_hook_info_set__func_m2283E27CCE511047C4028072A7FCC50E1354A281 (commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* __this, delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* ___0_value, const RuntimeMethod* method) 
{
	{
		delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* L_0 = ___0_value;
		__this->___U3C_funcU3Ek__BackingField = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3C_funcU3Ek__BackingField), (void*)L_0);
		return;
	}
}
// Method Definition Index: 117739
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* commit_hook_info_get__user_data_mD65F8F042015B81A6FD9D5CB216FF346F0AB68C6 (commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* __this, const RuntimeMethod* method) 
{
	{
		RuntimeObject* L_0 = __this->___U3C_user_dataU3Ek__BackingField;
		return L_0;
	}
}
// Method Definition Index: 117740
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void commit_hook_info_set__user_data_mA2C1CEA31C422E0678A0176851D9623090CFE266 (commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* __this, RuntimeObject* ___0_value, const RuntimeMethod* method) 
{
	{
		RuntimeObject* L_0 = ___0_value;
		__this->___U3C_user_dataU3Ek__BackingField = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3C_user_dataU3Ek__BackingField), (void*)L_0);
		return;
	}
}
// Method Definition Index: 117741
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void commit_hook_info__ctor_m4B4EC8FA983EECCEF944C38F087A910E4B85C74C (commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* __this, delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* ___0_func, RuntimeObject* ___1_v, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* L_0 = ___0_func;
		commit_hook_info_set__func_m2283E27CCE511047C4028072A7FCC50E1354A281_inline(__this, L_0, NULL);
		RuntimeObject* L_1 = ___1_v;
		commit_hook_info_set__user_data_mA2C1CEA31C422E0678A0176851D9623090CFE266_inline(__this, L_1, NULL);
		return;
	}
}
// Method Definition Index: 117742
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t commit_hook_info_call_mE9C755CD3924D7327451B4E46477242A86EED90A (commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* __this, const RuntimeMethod* method) 
{
	{
		delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* L_0;
		L_0 = commit_hook_info_get__func_m05BEEED3611D2441FE7BB1D3FA2DC7B53F14B46D_inline(__this, NULL);
		RuntimeObject* L_1;
		L_1 = commit_hook_info_get__user_data_mD65F8F042015B81A6FD9D5CB216FF346F0AB68C6_inline(__this, NULL);
		NullCheck(L_0);
		int32_t L_2;
		L_2 = delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_inline(L_0, L_1, NULL);
		return L_2;
	}
}
// Method Definition Index: 117743
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* commit_hook_info_from_ptr_m148F08D6E0B637E71B38D49209199AFCD2C73732 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___0_p;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_1;
		L_1 = GCHandle_op_Explicit_mA5F28206637454AD677BE13DF86C6152190B6F0F(L_0, NULL);
		V_0 = L_1;
		RuntimeObject* L_2;
		L_2 = GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_inline((&V_0), NULL);
		return ((commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78*)IsInstClass((RuntimeObject*)L_2, commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78_il2cpp_TypeInfo_var));
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117744
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void rollback_hook_info__ctor_m13E17137A92C3596FED3261983A9BB4C0ECEF32E (rollback_hook_info_t413A1090EF5A7F4DC0305C94AAA26DB193291185* __this, delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* ___0_func, RuntimeObject* ___1_v, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* L_0 = ___0_func;
		__this->____func = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____func), (void*)L_0);
		RuntimeObject* L_1 = ___1_v;
		__this->____user_data = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____user_data), (void*)L_1);
		return;
	}
}
// Method Definition Index: 117745
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR rollback_hook_info_t413A1090EF5A7F4DC0305C94AAA26DB193291185* rollback_hook_info_from_ptr_mFE4504F4B5F5151FA11537A355453CD824D5CCB7 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&rollback_hook_info_t413A1090EF5A7F4DC0305C94AAA26DB193291185_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___0_p;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_1;
		L_1 = GCHandle_op_Explicit_mA5F28206637454AD677BE13DF86C6152190B6F0F(L_0, NULL);
		V_0 = L_1;
		RuntimeObject* L_2;
		L_2 = GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_inline((&V_0), NULL);
		return ((rollback_hook_info_t413A1090EF5A7F4DC0305C94AAA26DB193291185*)IsInstClass((RuntimeObject*)L_2, rollback_hook_info_t413A1090EF5A7F4DC0305C94AAA26DB193291185_il2cpp_TypeInfo_var));
	}
}
// Method Definition Index: 117746
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void rollback_hook_info_call_m8CC5F80155D547D5AB32D3C9737E8F0F5E410353 (rollback_hook_info_t413A1090EF5A7F4DC0305C94AAA26DB193291185* __this, const RuntimeMethod* method) 
{
	{
		delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* L_0 = __this->____func;
		RuntimeObject* L_1 = __this->____user_data;
		NullCheck(L_0);
		delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_inline(L_0, L_1, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117747
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void trace_hook_info__ctor_mA70153D94F7B356F1C59FFF1F66037EC470FA9D0 (trace_hook_info_t4C641DC6C4FE4BFDFACF1B300BA0A794A91A53F4* __this, delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* ___0_func, RuntimeObject* ___1_v, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* L_0 = ___0_func;
		__this->____func = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____func), (void*)L_0);
		RuntimeObject* L_1 = ___1_v;
		__this->____user_data = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____user_data), (void*)L_1);
		return;
	}
}
// Method Definition Index: 117748
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR trace_hook_info_t4C641DC6C4FE4BFDFACF1B300BA0A794A91A53F4* trace_hook_info_from_ptr_mE42166E00F3CD4951400F295150C299A55FDC8B8 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&trace_hook_info_t4C641DC6C4FE4BFDFACF1B300BA0A794A91A53F4_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___0_p;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_1;
		L_1 = GCHandle_op_Explicit_mA5F28206637454AD677BE13DF86C6152190B6F0F(L_0, NULL);
		V_0 = L_1;
		RuntimeObject* L_2;
		L_2 = GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_inline((&V_0), NULL);
		return ((trace_hook_info_t4C641DC6C4FE4BFDFACF1B300BA0A794A91A53F4*)IsInstClass((RuntimeObject*)L_2, trace_hook_info_t4C641DC6C4FE4BFDFACF1B300BA0A794A91A53F4_il2cpp_TypeInfo_var));
	}
}
// Method Definition Index: 117749
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void trace_hook_info_call_mA98E6A93D09DD73D282B0C0C657CBF0DBF741B00 (trace_hook_info_t4C641DC6C4FE4BFDFACF1B300BA0A794A91A53F4* __this, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___0_s, const RuntimeMethod* method) 
{
	{
		delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* L_0 = __this->____func;
		RuntimeObject* L_1 = __this->____user_data;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___0_s;
		NullCheck(L_0);
		delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_inline(L_0, L_1, L_2, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117750
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void profile_hook_info__ctor_mA79CFEE7D02660A458390BF3C4218C78E22E679E (profile_hook_info_tE9263823781920B9D354CC58CF351F325EDF3D34* __this, delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* ___0_func, RuntimeObject* ___1_v, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* L_0 = ___0_func;
		__this->____func = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____func), (void*)L_0);
		RuntimeObject* L_1 = ___1_v;
		__this->____user_data = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____user_data), (void*)L_1);
		return;
	}
}
// Method Definition Index: 117751
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR profile_hook_info_tE9263823781920B9D354CC58CF351F325EDF3D34* profile_hook_info_from_ptr_m3A176B51EBEA07D80FC850F2C169109E7DE1A940 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&profile_hook_info_tE9263823781920B9D354CC58CF351F325EDF3D34_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___0_p;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_1;
		L_1 = GCHandle_op_Explicit_mA5F28206637454AD677BE13DF86C6152190B6F0F(L_0, NULL);
		V_0 = L_1;
		RuntimeObject* L_2;
		L_2 = GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_inline((&V_0), NULL);
		return ((profile_hook_info_tE9263823781920B9D354CC58CF351F325EDF3D34*)IsInstClass((RuntimeObject*)L_2, profile_hook_info_tE9263823781920B9D354CC58CF351F325EDF3D34_il2cpp_TypeInfo_var));
	}
}
// Method Definition Index: 117752
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void profile_hook_info_call_m438C34143623CA589DC456BFEBD6EC5499E4BDB0 (profile_hook_info_tE9263823781920B9D354CC58CF351F325EDF3D34* __this, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___0_s, int64_t ___1_elapsed, const RuntimeMethod* method) 
{
	{
		delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* L_0 = __this->____func;
		RuntimeObject* L_1 = __this->____user_data;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_2 = ___0_s;
		int64_t L_3 = ___1_elapsed;
		NullCheck(L_0);
		delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_inline(L_0, L_1, L_2, L_3, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117753
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void progress_hook_info__ctor_mD9BE365FBD60098A96229B0A7FE561438372B947 (progress_hook_info_tF0C73E17649E97704B133D3007AD666D3B416641* __this, delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* ___0_func, RuntimeObject* ___1_v, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* L_0 = ___0_func;
		__this->____func = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____func), (void*)L_0);
		RuntimeObject* L_1 = ___1_v;
		__this->____user_data = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____user_data), (void*)L_1);
		return;
	}
}
// Method Definition Index: 117754
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR progress_hook_info_tF0C73E17649E97704B133D3007AD666D3B416641* progress_hook_info_from_ptr_m5367E1B00176E2D1A5CF6BCAA4B54159BB178494 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&progress_hook_info_tF0C73E17649E97704B133D3007AD666D3B416641_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___0_p;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_1;
		L_1 = GCHandle_op_Explicit_mA5F28206637454AD677BE13DF86C6152190B6F0F(L_0, NULL);
		V_0 = L_1;
		RuntimeObject* L_2;
		L_2 = GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_inline((&V_0), NULL);
		return ((progress_hook_info_tF0C73E17649E97704B133D3007AD666D3B416641*)IsInstClass((RuntimeObject*)L_2, progress_hook_info_tF0C73E17649E97704B133D3007AD666D3B416641_il2cpp_TypeInfo_var));
	}
}
// Method Definition Index: 117755
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t progress_hook_info_call_m1DBC36FDEF18460447F83230864D87083BCD6C16 (progress_hook_info_tF0C73E17649E97704B133D3007AD666D3B416641* __this, const RuntimeMethod* method) 
{
	{
		delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* L_0 = __this->____func;
		RuntimeObject* L_1 = __this->____user_data;
		NullCheck(L_0);
		int32_t L_2;
		L_2 = delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_inline(L_0, L_1, NULL);
		return L_2;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117756
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void update_hook_info__ctor_mA3635FE2E96A5676FE6834AE74D6E18ACBE5206E (update_hook_info_tC07EBFD924B99F90D87267BC47281F4AC1098968* __this, delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* ___0_func, RuntimeObject* ___1_v, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* L_0 = ___0_func;
		__this->____func = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____func), (void*)L_0);
		RuntimeObject* L_1 = ___1_v;
		__this->____user_data = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____user_data), (void*)L_1);
		return;
	}
}
// Method Definition Index: 117757
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR update_hook_info_tC07EBFD924B99F90D87267BC47281F4AC1098968* update_hook_info_from_ptr_mE288FBF47CDEC3705ABBBD89202E5426CF7A2084 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&update_hook_info_tC07EBFD924B99F90D87267BC47281F4AC1098968_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___0_p;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_1;
		L_1 = GCHandle_op_Explicit_mA5F28206637454AD677BE13DF86C6152190B6F0F(L_0, NULL);
		V_0 = L_1;
		RuntimeObject* L_2;
		L_2 = GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_inline((&V_0), NULL);
		return ((update_hook_info_tC07EBFD924B99F90D87267BC47281F4AC1098968*)IsInstClass((RuntimeObject*)L_2, update_hook_info_tC07EBFD924B99F90D87267BC47281F4AC1098968_il2cpp_TypeInfo_var));
	}
}
// Method Definition Index: 117758
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void update_hook_info_call_mAF608749DC3EC4FD8EFAF0D535E17C0E99EE5D7F (update_hook_info_tC07EBFD924B99F90D87267BC47281F4AC1098968* __this, int32_t ___0_typ, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_db, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_tbl, int64_t ___3_rowid, const RuntimeMethod* method) 
{
	{
		delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* L_0 = __this->____func;
		RuntimeObject* L_1 = __this->____user_data;
		int32_t L_2 = ___0_typ;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3 = ___1_db;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_4 = ___2_tbl;
		int64_t L_5 = ___3_rowid;
		NullCheck(L_0);
		delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_inline(L_0, L_1, L_2, L_3, L_4, L_5, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117759
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void collation_hook_info__ctor_mDB95BB4828C4E65CB57C4BCEE0516A06E6B1CB0A (collation_hook_info_tDD9E78A2C2512E1FEF753347B75A4DEC251D9E98* __this, delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* ___0_func, RuntimeObject* ___1_v, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* L_0 = ___0_func;
		__this->____func = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____func), (void*)L_0);
		RuntimeObject* L_1 = ___1_v;
		__this->____user_data = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____user_data), (void*)L_1);
		return;
	}
}
// Method Definition Index: 117760
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR collation_hook_info_tDD9E78A2C2512E1FEF753347B75A4DEC251D9E98* collation_hook_info_from_ptr_mADF5398D8E60D65B942882B28C339CA80ABEF966 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&collation_hook_info_tDD9E78A2C2512E1FEF753347B75A4DEC251D9E98_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___0_p;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_1;
		L_1 = GCHandle_op_Explicit_mA5F28206637454AD677BE13DF86C6152190B6F0F(L_0, NULL);
		V_0 = L_1;
		RuntimeObject* L_2;
		L_2 = GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_inline((&V_0), NULL);
		return ((collation_hook_info_tDD9E78A2C2512E1FEF753347B75A4DEC251D9E98*)IsInstClass((RuntimeObject*)L_2, collation_hook_info_tDD9E78A2C2512E1FEF753347B75A4DEC251D9E98_il2cpp_TypeInfo_var));
	}
}
// Method Definition Index: 117761
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t collation_hook_info_call_mF9F263DD935E37DD890058669056336B8ED6E178 (collation_hook_info_tDD9E78A2C2512E1FEF753347B75A4DEC251D9E98* __this, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___0_s1, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_s2, const RuntimeMethod* method) 
{
	{
		delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* L_0 = __this->____func;
		RuntimeObject* L_1 = __this->____user_data;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_2 = ___0_s1;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_3 = ___1_s2;
		NullCheck(L_0);
		int32_t L_4;
		L_4 = delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_inline(L_0, L_1, L_2, L_3, NULL);
		return L_4;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117762
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void exec_hook_info__ctor_mF44B73FC081E66935ACBC8C8635D85C9E4D3ED24 (exec_hook_info_tB2C6835FA07DBFA56D9C701D6A2C0FC6D9201CA8* __this, delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* ___0_func, RuntimeObject* ___1_v, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* L_0 = ___0_func;
		__this->____func = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____func), (void*)L_0);
		RuntimeObject* L_1 = ___1_v;
		__this->____user_data = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____user_data), (void*)L_1);
		return;
	}
}
// Method Definition Index: 117763
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR exec_hook_info_tB2C6835FA07DBFA56D9C701D6A2C0FC6D9201CA8* exec_hook_info_from_ptr_m12E3B2B744FC54C5D4743771EAA23288615B4BDF (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&exec_hook_info_tB2C6835FA07DBFA56D9C701D6A2C0FC6D9201CA8_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___0_p;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_1;
		L_1 = GCHandle_op_Explicit_mA5F28206637454AD677BE13DF86C6152190B6F0F(L_0, NULL);
		V_0 = L_1;
		RuntimeObject* L_2;
		L_2 = GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_inline((&V_0), NULL);
		return ((exec_hook_info_tB2C6835FA07DBFA56D9C701D6A2C0FC6D9201CA8*)IsInstClass((RuntimeObject*)L_2, exec_hook_info_tB2C6835FA07DBFA56D9C701D6A2C0FC6D9201CA8_il2cpp_TypeInfo_var));
	}
}
// Method Definition Index: 117764
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t exec_hook_info_call_mA713B5044B9732EB1EE735288166504122BFCA77 (exec_hook_info_tB2C6835FA07DBFA56D9C701D6A2C0FC6D9201CA8* __this, int32_t ___0_n, intptr_t ___1_values_ptr, intptr_t ___2_names_ptr, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* V_0 = NULL;
	IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* V_1 = NULL;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	intptr_t V_4;
	memset((&V_4), 0, sizeof(V_4));
	{
		int32_t L_0 = ___0_n;
		IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* L_1 = (IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*)(IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*)SZArrayNew(IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832_il2cpp_TypeInfo_var, (uint32_t)L_0);
		V_0 = L_1;
		int32_t L_2 = ___0_n;
		IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* L_3 = (IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*)(IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*)SZArrayNew(IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832_il2cpp_TypeInfo_var, (uint32_t)L_2);
		V_1 = L_3;
		RuntimeTypeHandle_t332A452B8B6179E4469B69525D0FE82A88030F7B L_4 = { reinterpret_cast<intptr_t> (&il2cpp_defaults.int_class->byval_arg) };
		il2cpp_codegen_runtime_class_init_inline(il2cpp_defaults.systemtype_class);
		Type_t* L_5;
		L_5 = Type_GetTypeFromHandle_m6062B81682F79A4D6DF2640692EE6D9987858C57(L_4, NULL);
		il2cpp_codegen_runtime_class_init_inline(Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		int32_t L_6;
		L_6 = Marshal_SizeOf_mED64846722033D6F60C2973CA604B7C2D7D4A1B7(L_5, NULL);
		V_2 = L_6;
		V_3 = 0;
		goto IL_0046;
	}

IL_0022:
	{
		intptr_t L_7 = ___1_values_ptr;
		int32_t L_8 = V_3;
		int32_t L_9 = V_2;
		il2cpp_codegen_runtime_class_init_inline(Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		intptr_t L_10;
		L_10 = Marshal_ReadIntPtr_m576E200A849BE7A6BC688058AA869B12B30D970F(L_7, ((int32_t)il2cpp_codegen_multiply(L_8, L_9)), NULL);
		V_4 = L_10;
		IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* L_11 = V_0;
		int32_t L_12 = V_3;
		intptr_t L_13 = V_4;
		NullCheck(L_11);
		(L_11)->SetAt(static_cast<il2cpp_array_size_t>(L_12), (intptr_t)L_13);
		intptr_t L_14 = ___2_names_ptr;
		int32_t L_15 = V_3;
		int32_t L_16 = V_2;
		intptr_t L_17;
		L_17 = Marshal_ReadIntPtr_m576E200A849BE7A6BC688058AA869B12B30D970F(L_14, ((int32_t)il2cpp_codegen_multiply(L_15, L_16)), NULL);
		V_4 = L_17;
		IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* L_18 = V_1;
		int32_t L_19 = V_3;
		intptr_t L_20 = V_4;
		NullCheck(L_18);
		(L_18)->SetAt(static_cast<il2cpp_array_size_t>(L_19), (intptr_t)L_20);
		int32_t L_21 = V_3;
		V_3 = ((int32_t)il2cpp_codegen_add(L_21, 1));
	}

IL_0046:
	{
		int32_t L_22 = V_3;
		int32_t L_23 = ___0_n;
		if ((((int32_t)L_22) < ((int32_t)L_23)))
		{
			goto IL_0022;
		}
	}
	{
		delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* L_24 = __this->____func;
		RuntimeObject* L_25 = __this->____user_data;
		IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* L_26 = V_0;
		IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* L_27 = V_1;
		NullCheck(L_24);
		int32_t L_28;
		L_28 = delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_inline(L_24, L_25, L_26, L_27, NULL);
		return L_28;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117765
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void function_hook_info__ctor_m4B1D6B0D2A0FF53B36B2BF437FD87A3BB8B20E86 (function_hook_info_t2884BE4D2AF455A249E6382005E52E62CDB7E017* __this, delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* ___0_func_scalar, RuntimeObject* ___1_user_data, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* L_0 = ___0_func_scalar;
		__this->____func_scalar = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____func_scalar), (void*)L_0);
		RuntimeObject* L_1 = ___1_user_data;
		__this->____user_data = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____user_data), (void*)L_1);
		return;
	}
}
// Method Definition Index: 117766
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void function_hook_info__ctor_mD469D8E11284B583FA130AFD16FB2EBACB59FDF2 (function_hook_info_t2884BE4D2AF455A249E6382005E52E62CDB7E017* __this, delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* ___0_func_step, delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* ___1_func_final, RuntimeObject* ___2_user_data, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* L_0 = ___0_func_step;
		__this->____func_step = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____func_step), (void*)L_0);
		delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* L_1 = ___1_func_final;
		__this->____func_final = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____func_final), (void*)L_1);
		RuntimeObject* L_2 = ___2_user_data;
		__this->____user_data = L_2;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____user_data), (void*)L_2);
		return;
	}
}
// Method Definition Index: 117767
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR function_hook_info_t2884BE4D2AF455A249E6382005E52E62CDB7E017* function_hook_info_from_ptr_mCA53C85ACFB80A6A189E7CA538BE44E0D400F923 (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&function_hook_info_t2884BE4D2AF455A249E6382005E52E62CDB7E017_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___0_p;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_1;
		L_1 = GCHandle_op_Explicit_mA5F28206637454AD677BE13DF86C6152190B6F0F(L_0, NULL);
		V_0 = L_1;
		RuntimeObject* L_2;
		L_2 = GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_inline((&V_0), NULL);
		return ((function_hook_info_t2884BE4D2AF455A249E6382005E52E62CDB7E017*)IsInstClass((RuntimeObject*)L_2, function_hook_info_t2884BE4D2AF455A249E6382005E52E62CDB7E017_il2cpp_TypeInfo_var));
	}
}
// Method Definition Index: 117768
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* function_hook_info_get_context_mAB02E6F7CA2611530E05A39863DBD6FE1CF323B5 (function_hook_info_t2884BE4D2AF455A249E6382005E52E62CDB7E017* __this, intptr_t ___0_context, intptr_t ___1_agg_context, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07* V_0 = NULL;
	intptr_t V_1;
	memset((&V_1), 0, sizeof(V_1));
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_2;
	memset((&V_2), 0, sizeof(V_2));
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_3;
	memset((&V_3), 0, sizeof(V_3));
	{
		intptr_t L_0 = ___1_agg_context;
		il2cpp_codegen_runtime_class_init_inline(Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		intptr_t L_1;
		L_1 = Marshal_ReadIntPtr_m6E8694E5CB4FE576B3CAE1A002B03C211D393826(L_0, NULL);
		V_1 = L_1;
		intptr_t L_2 = V_1;
		bool L_3;
		L_3 = IntPtr_op_Equality_m7D9CDCDE9DC2A0C2C614633F4921E90187FAB271_inline(L_2, 0, NULL);
		if (!L_3)
		{
			goto IL_0035;
		}
	}
	{
		RuntimeObject* L_4 = __this->____user_data;
		agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07* L_5 = (agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07*)il2cpp_codegen_object_new(agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07_il2cpp_TypeInfo_var);
		agg_sqlite3_context__ctor_m2FD4EE33CC38CC17ADF6466615FE093912A8C168(L_5, L_4, NULL);
		V_0 = L_5;
		agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07* L_6 = V_0;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_7;
		L_7 = GCHandle_Alloc_m845AB5ED62859B099C023F34C05BEAEDB4AFE27D(L_6, NULL);
		V_2 = L_7;
		intptr_t L_8 = ___1_agg_context;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_9 = V_2;
		intptr_t L_10;
		L_10 = GCHandle_op_Explicit_m03DD8D9FB45D565431455A6EE5C30A87305EF73C_inline(L_9, NULL);
		il2cpp_codegen_runtime_class_init_inline(Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		Marshal_WriteIntPtr_m3AA18248A64282B1CFB4FF0B13678B2E08DADA36(L_8, L_10, NULL);
		goto IL_0049;
	}

IL_0035:
	{
		intptr_t L_11 = V_1;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_12;
		L_12 = GCHandle_op_Explicit_mA5F28206637454AD677BE13DF86C6152190B6F0F(L_11, NULL);
		V_3 = L_12;
		RuntimeObject* L_13;
		L_13 = GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_inline((&V_3), NULL);
		V_0 = ((agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07*)IsInstClass((RuntimeObject*)L_13, agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07_il2cpp_TypeInfo_var));
	}

IL_0049:
	{
		agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07* L_14 = V_0;
		intptr_t L_15 = ___0_context;
		NullCheck(L_14);
		agg_sqlite3_context_fix_ptr_m1A20B24DEF5D9E1199E5A60A31376373947B024E(L_14, L_15, NULL);
		agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07* L_16 = V_0;
		return L_16;
	}
}
// Method Definition Index: 117769
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void function_hook_info_call_scalar_mAA7975A8E3B3EDBE7C1FCDFC02178C7F72DDD7BB (function_hook_info_t2884BE4D2AF455A249E6382005E52E62CDB7E017* __this, intptr_t ___0_context, int32_t ___1_num_args, intptr_t ___2_argsptr, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&scalar_sqlite3_context_t520AE3E6198561DEF6B32657E820E9B8FBDE0509_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	scalar_sqlite3_context_t520AE3E6198561DEF6B32657E820E9B8FBDE0509* V_0 = NULL;
	sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* V_1 = NULL;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	intptr_t V_4;
	memset((&V_4), 0, sizeof(V_4));
	{
		intptr_t L_0 = ___0_context;
		RuntimeObject* L_1 = __this->____user_data;
		scalar_sqlite3_context_t520AE3E6198561DEF6B32657E820E9B8FBDE0509* L_2 = (scalar_sqlite3_context_t520AE3E6198561DEF6B32657E820E9B8FBDE0509*)il2cpp_codegen_object_new(scalar_sqlite3_context_t520AE3E6198561DEF6B32657E820E9B8FBDE0509_il2cpp_TypeInfo_var);
		scalar_sqlite3_context__ctor_mE4C296D9999B1489512214B36C283EFBBA65B72F(L_2, L_0, L_1, NULL);
		V_0 = L_2;
		int32_t L_3 = ___1_num_args;
		sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* L_4 = (sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894*)(sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894*)SZArrayNew(sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894_il2cpp_TypeInfo_var, (uint32_t)L_3);
		V_1 = L_4;
		RuntimeTypeHandle_t332A452B8B6179E4469B69525D0FE82A88030F7B L_5 = { reinterpret_cast<intptr_t> (&il2cpp_defaults.int_class->byval_arg) };
		il2cpp_codegen_runtime_class_init_inline(il2cpp_defaults.systemtype_class);
		Type_t* L_6;
		L_6 = Type_GetTypeFromHandle_m6062B81682F79A4D6DF2640692EE6D9987858C57(L_5, NULL);
		il2cpp_codegen_runtime_class_init_inline(Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		int32_t L_7;
		L_7 = Marshal_SizeOf_mED64846722033D6F60C2973CA604B7C2D7D4A1B7(L_6, NULL);
		V_2 = L_7;
		V_3 = 0;
		goto IL_0041;
	}

IL_0028:
	{
		intptr_t L_8 = ___2_argsptr;
		int32_t L_9 = V_3;
		int32_t L_10 = V_2;
		il2cpp_codegen_runtime_class_init_inline(Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		intptr_t L_11;
		L_11 = Marshal_ReadIntPtr_m576E200A849BE7A6BC688058AA869B12B30D970F(L_8, ((int32_t)il2cpp_codegen_multiply(L_9, L_10)), NULL);
		V_4 = L_11;
		sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* L_12 = V_1;
		int32_t L_13 = V_3;
		intptr_t L_14 = V_4;
		sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* L_15 = (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD*)il2cpp_codegen_object_new(sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD_il2cpp_TypeInfo_var);
		sqlite3_value__ctor_m653B23B5B2873FC7E8AB166AAE5E00151A929676(L_15, L_14, NULL);
		NullCheck(L_12);
		ArrayElementTypeCheck (L_12, L_15);
		(L_12)->SetAt(static_cast<il2cpp_array_size_t>(L_13), (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD*)L_15);
		int32_t L_16 = V_3;
		V_3 = ((int32_t)il2cpp_codegen_add(L_16, 1));
	}

IL_0041:
	{
		int32_t L_17 = V_3;
		int32_t L_18 = ___1_num_args;
		if ((((int32_t)L_17) < ((int32_t)L_18)))
		{
			goto IL_0028;
		}
	}
	{
		delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* L_19 = __this->____func_scalar;
		scalar_sqlite3_context_t520AE3E6198561DEF6B32657E820E9B8FBDE0509* L_20 = V_0;
		RuntimeObject* L_21 = __this->____user_data;
		sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* L_22 = V_1;
		NullCheck(L_19);
		delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_inline(L_19, L_20, L_21, L_22, NULL);
		return;
	}
}
// Method Definition Index: 117770
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void function_hook_info_call_step_m50C9070AFF7A909228C5C298AE3CB348571F9FF9 (function_hook_info_t2884BE4D2AF455A249E6382005E52E62CDB7E017* __this, intptr_t ___0_context, intptr_t ___1_agg_context, int32_t ___2_num_args, intptr_t ___3_argsptr, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* V_0 = NULL;
	sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* V_1 = NULL;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	intptr_t V_4;
	memset((&V_4), 0, sizeof(V_4));
	{
		intptr_t L_0 = ___0_context;
		intptr_t L_1 = ___1_agg_context;
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_2;
		L_2 = function_hook_info_get_context_mAB02E6F7CA2611530E05A39863DBD6FE1CF323B5(__this, L_0, L_1, NULL);
		V_0 = L_2;
		int32_t L_3 = ___2_num_args;
		sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* L_4 = (sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894*)(sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894*)SZArrayNew(sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894_il2cpp_TypeInfo_var, (uint32_t)L_3);
		V_1 = L_4;
		RuntimeTypeHandle_t332A452B8B6179E4469B69525D0FE82A88030F7B L_5 = { reinterpret_cast<intptr_t> (&il2cpp_defaults.int_class->byval_arg) };
		il2cpp_codegen_runtime_class_init_inline(il2cpp_defaults.systemtype_class);
		Type_t* L_6;
		L_6 = Type_GetTypeFromHandle_m6062B81682F79A4D6DF2640692EE6D9987858C57(L_5, NULL);
		il2cpp_codegen_runtime_class_init_inline(Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		int32_t L_7;
		L_7 = Marshal_SizeOf_mED64846722033D6F60C2973CA604B7C2D7D4A1B7(L_6, NULL);
		V_2 = L_7;
		V_3 = 0;
		goto IL_003e;
	}

IL_0024:
	{
		intptr_t L_8 = ___3_argsptr;
		int32_t L_9 = V_3;
		int32_t L_10 = V_2;
		il2cpp_codegen_runtime_class_init_inline(Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		intptr_t L_11;
		L_11 = Marshal_ReadIntPtr_m576E200A849BE7A6BC688058AA869B12B30D970F(L_8, ((int32_t)il2cpp_codegen_multiply(L_9, L_10)), NULL);
		V_4 = L_11;
		sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* L_12 = V_1;
		int32_t L_13 = V_3;
		intptr_t L_14 = V_4;
		sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* L_15 = (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD*)il2cpp_codegen_object_new(sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD_il2cpp_TypeInfo_var);
		sqlite3_value__ctor_m653B23B5B2873FC7E8AB166AAE5E00151A929676(L_15, L_14, NULL);
		NullCheck(L_12);
		ArrayElementTypeCheck (L_12, L_15);
		(L_12)->SetAt(static_cast<il2cpp_array_size_t>(L_13), (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD*)L_15);
		int32_t L_16 = V_3;
		V_3 = ((int32_t)il2cpp_codegen_add(L_16, 1));
	}

IL_003e:
	{
		int32_t L_17 = V_3;
		int32_t L_18 = ___2_num_args;
		if ((((int32_t)L_17) < ((int32_t)L_18)))
		{
			goto IL_0024;
		}
	}
	{
		delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* L_19 = __this->____func_step;
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_20 = V_0;
		RuntimeObject* L_21 = __this->____user_data;
		sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* L_22 = V_1;
		NullCheck(L_19);
		delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_inline(L_19, L_20, L_21, L_22, NULL);
		return;
	}
}
// Method Definition Index: 117771
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void function_hook_info_call_final_m41F22B0B1C88975CAE40BD2E8A8F2ACAB962C76E (function_hook_info_t2884BE4D2AF455A249E6382005E52E62CDB7E017* __this, intptr_t ___0_context, intptr_t ___1_agg_context, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* V_0 = NULL;
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		intptr_t L_0 = ___0_context;
		intptr_t L_1 = ___1_agg_context;
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_2;
		L_2 = function_hook_info_get_context_mAB02E6F7CA2611530E05A39863DBD6FE1CF323B5(__this, L_0, L_1, NULL);
		V_0 = L_2;
		delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* L_3 = __this->____func_final;
		sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* L_4 = V_0;
		RuntimeObject* L_5 = __this->____user_data;
		NullCheck(L_3);
		delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_inline(L_3, L_4, L_5, NULL);
		intptr_t L_6 = ___1_agg_context;
		il2cpp_codegen_runtime_class_init_inline(Marshal_tD976A56A90263C3CE2B780D4B1CADADE2E70B4A7_il2cpp_TypeInfo_var);
		intptr_t L_7;
		L_7 = Marshal_ReadIntPtr_m6E8694E5CB4FE576B3CAE1A002B03C211D393826(L_6, NULL);
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_8;
		L_8 = GCHandle_op_Explicit_mA5F28206637454AD677BE13DF86C6152190B6F0F(L_7, NULL);
		V_1 = L_8;
		GCHandle_Free_m1320A260E487EB1EA6D95F9E54BFFCB5A4EF83A3((&V_1), NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117772
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void agg_sqlite3_context__ctor_m2FD4EE33CC38CC17ADF6466615FE093912A8C168 (agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07* __this, RuntimeObject* ___0_v, const RuntimeMethod* method) 
{
	{
		RuntimeObject* L_0 = ___0_v;
		sqlite3_context__ctor_mD3A1FF371768B7E9C88EC86EFE32952B8D89D71F(__this, L_0, NULL);
		return;
	}
}
// Method Definition Index: 117773
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void agg_sqlite3_context_fix_ptr_m1A20B24DEF5D9E1199E5A60A31376373947B024E (agg_sqlite3_context_t7AF70002EDC01CAA770CE2E2C7DE817FEBCEBE07* __this, intptr_t ___0_p, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ___0_p;
		sqlite3_context_set_context_ptr_m75037F45D04497B679E79EF821D191289E5917A3_inline(__this, L_0, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117774
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void scalar_sqlite3_context__ctor_mE4C296D9999B1489512214B36C283EFBBA65B72F (scalar_sqlite3_context_t520AE3E6198561DEF6B32657E820E9B8FBDE0509* __this, intptr_t ___0_p, RuntimeObject* ___1_v, const RuntimeMethod* method) 
{
	{
		RuntimeObject* L_0 = ___1_v;
		sqlite3_context__ctor_mD3A1FF371768B7E9C88EC86EFE32952B8D89D71F(__this, L_0, NULL);
		intptr_t L_1 = ___0_p;
		sqlite3_context_set_context_ptr_m75037F45D04497B679E79EF821D191289E5917A3_inline(__this, L_1, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117775
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void authorizer_hook_info__ctor_m356DDFFD020722A4AB519EE7BB5513B56AF063B4 (authorizer_hook_info_t27B31C9940EEBF2F29DD86AED0B1A0A04487768A* __this, delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* ___0_func, RuntimeObject* ___1_v, const RuntimeMethod* method) 
{
	{
		Object__ctor_mE837C6B9FA8C6D5D109F4B2EC885D79919AC0EA2(__this, NULL);
		delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* L_0 = ___0_func;
		__this->____func = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____func), (void*)L_0);
		RuntimeObject* L_1 = ___1_v;
		__this->____user_data = L_1;
		Il2CppCodeGenWriteBarrier((void**)(&__this->____user_data), (void*)L_1);
		return;
	}
}
// Method Definition Index: 117776
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR authorizer_hook_info_t27B31C9940EEBF2F29DD86AED0B1A0A04487768A* authorizer_hook_info_from_ptr_m05FE8A7AE031944CFECB7FEAAD7E6FAD0A836C8C (intptr_t ___0_p, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&authorizer_hook_info_t27B31C9940EEBF2F29DD86AED0B1A0A04487768A_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___0_p;
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_1;
		L_1 = GCHandle_op_Explicit_mA5F28206637454AD677BE13DF86C6152190B6F0F(L_0, NULL);
		V_0 = L_1;
		RuntimeObject* L_2;
		L_2 = GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_inline((&V_0), NULL);
		return ((authorizer_hook_info_t27B31C9940EEBF2F29DD86AED0B1A0A04487768A*)IsInstClass((RuntimeObject*)L_2, authorizer_hook_info_t27B31C9940EEBF2F29DD86AED0B1A0A04487768A_il2cpp_TypeInfo_var));
	}
}
// Method Definition Index: 117777
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t authorizer_hook_info_call_mF1D0501AEBFE5DFE6FD3DA44EC327A13D0FADCBF (authorizer_hook_info_t27B31C9940EEBF2F29DD86AED0B1A0A04487768A* __this, int32_t ___0_action_code, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_param0, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_param1, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_dbName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___4_inner_most_trigger_or_view, const RuntimeMethod* method) 
{
	{
		delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* L_0 = __this->____func;
		RuntimeObject* L_1 = __this->____user_data;
		int32_t L_2 = ___0_action_code;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_3 = ___1_param0;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_4 = ___2_param1;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_5 = ___3_dbName;
		utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 L_6 = ___4_inner_most_trigger_or_view;
		NullCheck(L_0);
		int32_t L_7;
		L_7 = delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_inline(L_0, L_1, L_2, L_3, L_4, L_5, L_6, NULL);
		return L_7;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 117778
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* EntryPointAttribute_get_Name_mB690110C74A5EEC536B754756E3629051CEE6834 (EntryPointAttribute_t153F60F730CBAB2CE127F07F09554695F9B9D075* __this, const RuntimeMethod* method) 
{
	{
		String_t* L_0 = __this->___U3CNameU3Ek__BackingField;
		return L_0;
	}
}
// Method Definition Index: 117779
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EntryPointAttribute_set_Name_m865E8DAD340EDE93960B660C46F3CA0602439808 (EntryPointAttribute_t153F60F730CBAB2CE127F07F09554695F9B9D075* __this, String_t* ___0_value, const RuntimeMethod* method) 
{
	{
		String_t* L_0 = ___0_value;
		__this->___U3CNameU3Ek__BackingField = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3CNameU3Ek__BackingField), (void*)L_0);
		return;
	}
}
// Method Definition Index: 117780
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EntryPointAttribute__ctor_m7BA2A2ED5DA1D962B034DC1070BF85312E284359 (EntryPointAttribute_t153F60F730CBAB2CE127F07F09554695F9B9D075* __this, String_t* ___0_name, const RuntimeMethod* method) 
{
	{
		Attribute__ctor_m79ED1BF1EE36D1E417BA89A0D9F91F8AAD8D19E2(__this, NULL);
		String_t* L_0 = ___0_name;
		EntryPointAttribute_set_Name_m865E8DAD340EDE93960B660C46F3CA0602439808_inline(__this, L_0, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
// Method Definition Index: 25072
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool IntPtr_op_Equality_m7D9CDCDE9DC2A0C2C614633F4921E90187FAB271_inline (intptr_t ___0_value1, intptr_t ___1_value2, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ___0_value1;
		intptr_t L_1 = ___1_value2;
		return (bool)((((intptr_t)L_0) == ((intptr_t)L_1))? 1 : 0);
	}
}
// Method Definition Index: 117476
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* sqlite3_stmt_get_db_m31DCFD46B918941ED21CAD5F953201B59EF259CF_inline (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* __this, const RuntimeMethod* method) 
{
	{
		sqlite3_tE144FE5AD2EDE033B98BE41F3E6D40AE2469BEA2* L_0 = __this->____db;
		return L_0;
	}
}
// Method Definition Index: 117475
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR intptr_t sqlite3_stmt_get_ptr_mF5030B60EB110512D248A632A61AE2F899580B58_inline (sqlite3_stmt_t1D3FD1CE508E61E06BBEC8B87AA8785A0D1BF99F* __this, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ((SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7*)__this)->___handle;
		return L_0;
	}
}
// Method Definition Index: 117455
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject* sqlite3_context_get_user_data_m2B56AA945EBDDC7B032007BA993E2B0D6311E609_inline (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* __this, const RuntimeMethod* method) 
{
	{
		RuntimeObject* L_0 = __this->____user_data;
		return L_0;
	}
}
// Method Definition Index: 117456
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR intptr_t sqlite3_context_get_ptr_m12CD2A3168656A686B6D7455A6A8251F31149259_inline (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* __this, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = __this->____p;
		return L_0;
	}
}
// Method Definition Index: 117459
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR intptr_t sqlite3_value_get_ptr_mAF588DD3703B49533D61586741960A38A4515AD9_inline (sqlite3_value_t4BD1685B8485EB3F407B6A313AC87C00236FD4DD* __this, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = __this->____p;
		return L_0;
	}
}
// Method Definition Index: 21850
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t String_get_Length_m42625D67623FA5CC7A44D47425CE86FB946542D2_inline (String_t* __this, const RuntimeMethod* method) 
{
	{
		int32_t L_0 = __this->____stringLength;
		return L_0;
	}
}
// Method Definition Index: 117223
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void strdelegate_log_Invoke_m842E897F6C69BDB8575B290BD38D45D2BCE5836B_inline (strdelegate_log_t9801FBF160619DC2B734037012DD9A1EAEED29D1* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, String_t* ___2_msg, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, String_t*, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_errorCode, ___2_msg, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117231
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void strdelegate_trace_Invoke_m8AA89BE8E36E17DFE6DB85D1739714ECCA307779_inline (strdelegate_trace_t4473F983A242D2840BDD4294A62F9959A52E32D7* __this, RuntimeObject* ___0_user_data, String_t* ___1_s, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, String_t*, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_s, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117235
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void strdelegate_profile_Invoke_mAEAC18BB166E7E4AD12E832AB3271B5F627E80F4_inline (strdelegate_profile_t6EDF34607A42C7B65462A3297A727D1C74442D3C* __this, RuntimeObject* ___0_user_data, String_t* ___1_statement, int64_t ___2_ns, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, String_t*, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_statement, ___2_ns, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117219
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void strdelegate_update_Invoke_m6503D6CB91DE4001C843CC89EADA17C91C2C48B1_inline (strdelegate_update_t7749C43B6F3ED8734D7D5FE0A31A3654803A2886* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, String_t* ___2_database, String_t* ___3_table, int64_t ___4_rowid, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, String_t*, String_t*, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117215
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t strdelegate_collation_Invoke_mC35A71E4C150D418EACA88ED49C49E7BF24F5537_inline (strdelegate_collation_t494B6745B46C09D34DA973B6D3CA857C38457A1E* __this, RuntimeObject* ___0_user_data, String_t* ___1_s1, String_t* ___2_s2, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, String_t*, String_t*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_s1, ___2_s2, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117239
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t strdelegate_exec_Invoke_m0CA6282D89A624A4CD2ACC4D5B8C616524E11CA6_inline (strdelegate_exec_t463CF8B8A2625117546BC31DF4EE8F8F2000C583* __this, RuntimeObject* ___0_user_data, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___1_values, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___2_names, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*, StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_values, ___2_names, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117227
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t strdelegate_authorizer_Invoke_m0806118189372541E94DAFCB9B10B7D727322D34_inline (strdelegate_authorizer_t542E7D4A1FBD8A63121A57AA4AA928B3C11D2C29* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, String_t* ___2_param0, String_t* ___3_param1, String_t* ___4_dbName, String_t* ___5_inner_most_trigger_or_view, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, String_t*, String_t*, String_t*, String_t*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 29705
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void SafeHandle_SetHandle_m003D64748F9DFBA1E3C0B23798C23BA81AA21C2A_inline (SafeHandle_tC1A4DA80DA89B867CC011B707A07275230321BF7* __this, intptr_t ___0_handle, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ___0_handle;
		__this->___handle = L_0;
		return;
	}
}
// Method Definition Index: 25073
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool IntPtr_op_Inequality_m90EFC9C4CAD9A33E309F2DDF98EE4E1DD253637B_inline (intptr_t ___0_value1, intptr_t ___1_value2, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ___0_value1;
		intptr_t L_1 = ___1_value2;
		return (bool)((((int32_t)((((intptr_t)L_0) == ((intptr_t)L_1))? 1 : 0)) == ((int32_t)0))? 1 : 0);
	}
}
// Method Definition Index: 25069
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void* IntPtr_ToPointer_m1A0612EED3A1C8B8850BE2943CFC42523064B4F6_inline (intptr_t* __this, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = *__this;
		return (void*)(L_0);
	}
}
// Method Definition Index: 117698
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void utf8z__ctor_m4B9DBE66D43FBDB2E4AF2D6DAADBD85A87366FFD_inline (utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25* __this, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___0_a, const RuntimeMethod* method) 
{
	{
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_0 = ___0_a;
		__this->___sp = L_0;
		return;
	}
}
// Method Definition Index: 117719
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void FuncName_set_name_m2E558A596C0258BE6EE151C469BCD07A6426A937_inline (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_value, const RuntimeMethod* method) 
{
	{
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_0 = ___0_value;
		__this->___U3CnameU3Ek__BackingField = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3CnameU3Ek__BackingField), (void*)L_0);
		return;
	}
}
// Method Definition Index: 117721
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void FuncName_set_n_mBBC1FE0A2863C200708D2CE3BAB22423DA29259E_inline (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* __this, int32_t ___0_value, const RuntimeMethod* method) 
{
	{
		int32_t L_0 = ___0_value;
		__this->___U3CnU3Ek__BackingField = L_0;
		return;
	}
}
// Method Definition Index: 117720
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t FuncName_get_n_m91D9E17080DD4D1D59BD9879BE61EBCF5C88E0C4_inline (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* __this, const RuntimeMethod* method) 
{
	{
		int32_t L_0 = __this->___U3CnU3Ek__BackingField;
		return L_0;
	}
}
// Method Definition Index: 117718
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* FuncName_get_name_mB74437A157D3DF6F5D2A693DC4B2C5DC3E47D648_inline (FuncName_tFDE2AD1AF3A77477BB2CB603A71892EEDE9FA29A* __this, const RuntimeMethod* method) 
{
	{
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_0 = __this->___U3CnameU3Ek__BackingField;
		return L_0;
	}
}
// Method Definition Index: 29731
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject* GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_inline (GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC* __this, const RuntimeMethod* method) 
{
	{
		bool L_0;
		L_0 = GCHandle_get_IsAllocated_m241908103D8D867E11CCAB73C918729825E86843_inline(__this, NULL);
		if (L_0)
		{
			goto IL_0013;
		}
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_1 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_1, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral4EBC86E0EACFCA522AEB82874860D0E248D782A5)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_1, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&GCHandle_get_Target_m481F9508DA5E384D33CD1F4450060DC56BBD4CD5_RuntimeMethod_var)));
	}

IL_0013:
	{
		intptr_t L_2 = __this->___handle;
		bool L_3;
		L_3 = GCHandle_CanDereferenceHandle_mAAAC42D1268CEF3FDD040A3D1574773D08140579_inline(L_2, NULL);
		if (!L_3)
		{
			goto IL_002c;
		}
	}
	{
		intptr_t L_4 = __this->___handle;
		RuntimeObject* L_5;
		L_5 = GCHandle_GetRef_mAC7E58E62417209DC41C99F66BA70F0C3AA18DA8_inline(L_4, NULL);
		return L_5;
	}

IL_002c:
	{
		intptr_t L_6 = __this->___handle;
		RuntimeObject* L_7;
		L_7 = GCHandle_GetTarget_mE0AF851834410E2AEA6285B2497751570236C794(L_6, NULL);
		return L_7;
	}
}
// Method Definition Index: 117498
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_log_Invoke_m7F58C1CA179773A980ACF2CC9AA4126442C1DF19_inline (delegate_log_tE18367F8F9FEC29177DF17197EC57B54B04E5712* __this, RuntimeObject* ___0_user_data, int32_t ___1_errorCode, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_msg, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_errorCode, ___2_msg, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117738
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void commit_hook_info_set__func_m2283E27CCE511047C4028072A7FCC50E1354A281_inline (commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* __this, delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* ___0_value, const RuntimeMethod* method) 
{
	{
		delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* L_0 = ___0_value;
		__this->___U3C_funcU3Ek__BackingField = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3C_funcU3Ek__BackingField), (void*)L_0);
		return;
	}
}
// Method Definition Index: 117740
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void commit_hook_info_set__user_data_mA2C1CEA31C422E0678A0176851D9623090CFE266_inline (commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* __this, RuntimeObject* ___0_value, const RuntimeMethod* method) 
{
	{
		RuntimeObject* L_0 = ___0_value;
		__this->___U3C_user_dataU3Ek__BackingField = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3C_user_dataU3Ek__BackingField), (void*)L_0);
		return;
	}
}
// Method Definition Index: 117737
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* commit_hook_info_get__func_m05BEEED3611D2441FE7BB1D3FA2DC7B53F14B46D_inline (commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* __this, const RuntimeMethod* method) 
{
	{
		delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* L_0 = __this->___U3C_funcU3Ek__BackingField;
		return L_0;
	}
}
// Method Definition Index: 117739
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject* commit_hook_info_get__user_data_mD65F8F042015B81A6FD9D5CB216FF346F0AB68C6_inline (commit_hook_info_tB926B425FE1E7B690A3C92BB526B96E9C11E2E78* __this, const RuntimeMethod* method) 
{
	{
		RuntimeObject* L_0 = __this->___U3C_user_dataU3Ek__BackingField;
		return L_0;
	}
}
// Method Definition Index: 117510
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t delegate_commit_Invoke_mA8E52920E0CC7255ECB87873D46319CD422BBDD8_inline (delegate_commit_tFB9621864DD3318A8343B71F10623E409A6A551A* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117514
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_rollback_Invoke_mAA6288311B15CD014CAAF1A0EA7028AAEB2086C8_inline (delegate_rollback_t9C71A4F72D71A2E27A4B866E7496F8254CF08174* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117518
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_trace_Invoke_mEC4A798E165818F35389BE87682A93FB110F133E_inline (delegate_trace_t305C5668766EF69B59A7CA173C227301583E2172* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_statement, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117522
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_profile_Invoke_mA00F91E75A7BD01FE57F7D49D798C29C92A5C4C1_inline (delegate_profile_tBCB8F1F2018A12E6A5EB205454BFD82CA0182779* __this, RuntimeObject* ___0_user_data, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___1_statement, int64_t ___2_ns, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_statement, ___2_ns, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117526
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t delegate_progress_Invoke_mC2216C164A3D1DA376BB2433339EBBACF1549218_inline (delegate_progress_t2315CA638C6F11381A7046E279792FDCBC421710* __this, RuntimeObject* ___0_user_data, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117494
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_update_Invoke_mD0F246D71F98DCB5E6735541741EBFE040541349_inline (delegate_update_t56D212B3CC0976C795F39D25F683364DBF6ABC80* __this, RuntimeObject* ___0_user_data, int32_t ___1_type, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_database, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_table, int64_t ___4_rowid, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, int64_t, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_type, ___2_database, ___3_table, ___4_rowid, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117490
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t delegate_collation_Invoke_m2BAEC2B959C3F506D38F5FC7D71C13452664EEA8_inline (delegate_collation_t8DE04E00895A6B578A732989AFCE272264DF5CCF* __this, RuntimeObject* ___0_user_data, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___1_s1, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ___2_s2, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_s1, ___2_s2, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117506
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t delegate_exec_Invoke_m1B9FB2F758B87C294230C26E4B4D30AE2845AC0B_inline (delegate_exec_t41D7BC9F896A9FF945FC4548BFA2CB5B698D3AD3* __this, RuntimeObject* ___0_user_data, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___1_values, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___2_names, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*, IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832*, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_values, ___2_names, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 29737
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR intptr_t GCHandle_op_Explicit_m03DD8D9FB45D565431455A6EE5C30A87305EF73C_inline (GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC ___0_value, const RuntimeMethod* method) 
{
	{
		GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC L_0 = ___0_value;
		intptr_t L_1 = L_0.___handle;
		return L_1;
	}
}
// Method Definition Index: 117530
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_function_scalar_Invoke_m1434CA5165CF6781827D277E6C65480ADD67032D_inline (delegate_function_scalar_t6ACAAB11456A0DAC29290263706F889CF8639EE5* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894*, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_ctx, ___1_user_data, ___2_args, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117534
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_function_aggregate_step_Invoke_m8152DE1ACF18832F4D8D6E25968E5C2C64EA4511_inline (delegate_function_aggregate_step_t0FAD6EBC679FFDE154D560BFED981933489B6957* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894* ___2_args, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, sqlite3_valueU5BU5D_t8DE0FEFC735FE16A1877E152BA6750433C4C2894*, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_ctx, ___1_user_data, ___2_args, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117538
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void delegate_function_aggregate_final_Invoke_m5C3D2C71AE77D93A8F8AB9785DB37D28FC5DBD98_inline (delegate_function_aggregate_final_t8B6A92360745F4E49F100C9F709467A57C153FD0* __this, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* ___0_ctx, RuntimeObject* ___1_user_data, const RuntimeMethod* method) 
{
	typedef void (*FunctionPointerType) (RuntimeObject*, sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C*, RuntimeObject*, const RuntimeMethod*);
	((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_ctx, ___1_user_data, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117457
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void sqlite3_context_set_context_ptr_m75037F45D04497B679E79EF821D191289E5917A3_inline (sqlite3_context_tEC606503DE111D8B720AE90A4A514F7B8D99C37C* __this, intptr_t ___0_p, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ___0_p;
		__this->____p = L_0;
		return;
	}
}
// Method Definition Index: 117502
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t delegate_authorizer_Invoke_mC936893BDC1041434042F3CA020B0346C7EDF56F_inline (delegate_authorizer_tCD3D6AAEEEB5C24F8531B924B9317AD676C25933* __this, RuntimeObject* ___0_user_data, int32_t ___1_action_code, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___2_param0, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___3_param1, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___4_dbName, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25 ___5_inner_most_trigger_or_view, const RuntimeMethod* method) 
{
	typedef int32_t (*FunctionPointerType) (RuntimeObject*, RuntimeObject*, int32_t, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, utf8z_t829B6DD60834A606DBA0DFA173B38441D91E4E25, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_user_data, ___1_action_code, ___2_param0, ___3_param1, ___4_dbName, ___5_inner_most_trigger_or_view, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 117779
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void EntryPointAttribute_set_Name_m865E8DAD340EDE93960B660C46F3CA0602439808_inline (EntryPointAttribute_t153F60F730CBAB2CE127F07F09554695F9B9D075* __this, String_t* ___0_value, const RuntimeMethod* method) 
{
	{
		String_t* L_0 = ___0_value;
		__this->___U3CNameU3Ek__BackingField = L_0;
		Il2CppCodeGenWriteBarrier((void**)(&__this->___U3CNameU3Ek__BackingField), (void*)L_0);
		return;
	}
}
// Method Definition Index: 23547
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t ReadOnlySpan_1_get_Length_m54864A0BB817050A9110E85BB5FB31EF63699982_gshared_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, const RuntimeMethod* method) 
{
	{
		int32_t L_0 = __this->____length;
		return L_0;
	}
}
// Method Definition Index: 23534
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void ReadOnlySpan_1__ctor_m1D3E8C5A560BE65D9A5C3E5D0D891C79F4895B0B_gshared_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___0_array, const RuntimeMethod* method) 
{
	{
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_0 = ___0_array;
		if (L_0)
		{
			goto IL_000b;
		}
	}
	{
		il2cpp_codegen_initobj(__this, sizeof(ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D));
		return;
	}

IL_000b:
	{
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_1 = ___0_array;
		NullCheck((RuntimeArray*)L_1);
		uint8_t* L_2;
		L_2 = Array_GetRawSzArrayData_m2F8F5B2A381AEF971F12866D9C0A6C4FBA59F6BB_inline((RuntimeArray*)L_1, NULL);
		uint8_t* L_3;
		L_3 = il2cpp_unsafe_as_ref<uint8_t>(L_2);
		ByReference_1_t9C85BCCAAF8C525B6C06B07E922D8D217BE8D6FC L_4;
		memset((&L_4), 0, sizeof(L_4));
		il2cpp_codegen_by_reference_constructor((Il2CppByReference*)(&L_4), L_3);
		__this->____pointer = L_4;
		ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* L_5 = ___0_array;
		NullCheck(L_5);
		__this->____length = ((int32_t)(((RuntimeArray*)L_5)->max_length));
		return;
	}
}
// Method Definition Index: 23545
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D ReadOnlySpan_1_Slice_mEB3D3A427170FC5A0AB734619D4792C299697C89_gshared_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, int32_t ___0_start, int32_t ___1_length, const RuntimeMethod* method) 
{
	ByReference_1_t9C85BCCAAF8C525B6C06B07E922D8D217BE8D6FC V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		int32_t L_0 = ___0_start;
		int32_t L_1 = __this->____length;
		if ((!(((uint32_t)L_0) <= ((uint32_t)L_1))))
		{
			goto IL_0014;
		}
	}
	{
		int32_t L_2 = ___1_length;
		int32_t L_3 = __this->____length;
		int32_t L_4 = ___0_start;
		if ((!(((uint32_t)L_2) > ((uint32_t)((int32_t)il2cpp_codegen_subtract(L_3, L_4))))))
		{
			goto IL_0019;
		}
	}

IL_0014:
	{
		ThrowHelper_ThrowArgumentOutOfRangeException_mD7D90276EDCDF9394A8EA635923E3B48BB71BD56(NULL);
	}

IL_0019:
	{
		ByReference_1_t9C85BCCAAF8C525B6C06B07E922D8D217BE8D6FC L_5 = __this->____pointer;
		V_0 = L_5;
		uint8_t* L_6;
		L_6 = IL2CPP_BY_REFERENCE_GET_VALUE(uint8_t, (Il2CppByReference*)(&V_0));
		int32_t L_7 = ___0_start;
		uint8_t* L_8;
		L_8 = il2cpp_unsafe_add<uint8_t,int32_t>(L_6, L_7);
		int32_t L_9 = ___1_length;
		ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D L_10;
		memset((&L_10), 0, sizeof(L_10));
		ReadOnlySpan_1__ctor_m0FC0B92549C2968E80B5F75A85F28B96DBFCFD63_inline((&L_10), L_8, L_9, il2cpp_rgctx_method(InitializedTypeInfo(method->klass)->rgctx_data, 15));
		return L_10;
	}
}
// Method Definition Index: 23632
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void Span_1__ctor_mE18EBB601FBFA01BA29FE353364700952A9091FE_gshared_inline (Span_1_tDADAC65069DFE6B57C458109115ECD795ED39305* __this, void* ___0_pointer, int32_t ___1_length, const RuntimeMethod* method) 
{
	{
		goto IL_0016;
	}

IL_0016:
	{
		int32_t L_0 = ___1_length;
		if ((((int32_t)L_0) >= ((int32_t)0)))
		{
			goto IL_001f;
		}
	}
	{
		ThrowHelper_ThrowArgumentOutOfRangeException_mD7D90276EDCDF9394A8EA635923E3B48BB71BD56(NULL);
	}

IL_001f:
	{
		void* L_1 = ___0_pointer;
		uint8_t* L_2;
		L_2 = il2cpp_unsafe_as_ref<uint8_t>((uint8_t*)L_1);
		ByReference_1_t9C85BCCAAF8C525B6C06B07E922D8D217BE8D6FC L_3;
		memset((&L_3), 0, sizeof(L_3));
		il2cpp_codegen_by_reference_constructor((Il2CppByReference*)(&L_3), L_2);
		__this->____pointer = L_3;
		int32_t L_4 = ___1_length;
		__this->____length = L_4;
		return;
	}
}
// Method Definition Index: 23536
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void ReadOnlySpan_1__ctor_m470D1527EF015478E8677C7BCB52C8410A1DB604_gshared_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, void* ___0_pointer, int32_t ___1_length, const RuntimeMethod* method) 
{
	{
		goto IL_0016;
	}

IL_0016:
	{
		int32_t L_0 = ___1_length;
		if ((((int32_t)L_0) >= ((int32_t)0)))
		{
			goto IL_001f;
		}
	}
	{
		ThrowHelper_ThrowArgumentOutOfRangeException_mD7D90276EDCDF9394A8EA635923E3B48BB71BD56(NULL);
	}

IL_001f:
	{
		void* L_1 = ___0_pointer;
		uint8_t* L_2;
		L_2 = il2cpp_unsafe_as_ref<uint8_t>((uint8_t*)L_1);
		ByReference_1_t9C85BCCAAF8C525B6C06B07E922D8D217BE8D6FC L_3;
		memset((&L_3), 0, sizeof(L_3));
		il2cpp_codegen_by_reference_constructor((Il2CppByReference*)(&L_3), L_2);
		__this->____pointer = L_3;
		int32_t L_4 = ___1_length;
		__this->____length = L_4;
		return;
	}
}
// Method Definition Index: 22044
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool Func_4_Invoke_mF686DB15C7046521DFA2350715743471581C6580_gshared_inline (Func_4_t969FE6B8E28BD4E700C6BA2ED5F8794B88E37083* __this, intptr_t ___0_arg1, intptr_t ___1_arg2, int32_t ___2_arg3, const RuntimeMethod* method) 
{
	typedef bool (*FunctionPointerType) (RuntimeObject*, intptr_t, intptr_t, int32_t, const RuntimeMethod*);
	return ((FunctionPointerType)__this->___invoke_impl)((Il2CppObject*)__this->___method_code, ___0_arg1, ___1_arg2, ___2_arg3, reinterpret_cast<RuntimeMethod*>(__this->___method));
}
// Method Definition Index: 29727
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool GCHandle_get_IsAllocated_m241908103D8D867E11CCAB73C918729825E86843_inline (GCHandle_tC44F6F72EE68BD4CFABA24309DA7A179D41127DC* __this, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = __this->___handle;
		bool L_1;
		L_1 = IntPtr_op_Inequality_m90EFC9C4CAD9A33E309F2DDF98EE4E1DD253637B_inline(L_0, 0, NULL);
		return L_1;
	}
}
// Method Definition Index: 29730
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool GCHandle_CanDereferenceHandle_mAAAC42D1268CEF3FDD040A3D1574773D08140579_inline (intptr_t ___0_handle, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ___0_handle;
		return (bool)((((intptr_t)((intptr_t)(L_0&((intptr_t)1)))) == ((intptr_t)((intptr_t)0)))? 1 : 0);
	}
}
// Method Definition Index: 29728
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR RuntimeObject* GCHandle_GetRef_mAC7E58E62417209DC41C99F66BA70F0C3AA18DA8_inline (intptr_t ___0_handle, const RuntimeMethod* method) 
{
	{
		intptr_t L_0 = ___0_handle;
		void* L_1;
		L_1 = IntPtr_op_Explicit_m2728CBA081E79B97DDCF1D4FAD77B309CA1E94BF(L_0, NULL);
		RuntimeObject** L_2;
		L_2 = il2cpp_unsafe_as_ref<RuntimeObject*>((intptr_t*)L_1);
		RuntimeObject* L_3 = *((RuntimeObject**)L_2);
		return L_3;
	}
}
// Method Definition Index: 24391
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR uint8_t* Array_GetRawSzArrayData_m2F8F5B2A381AEF971F12866D9C0A6C4FBA59F6BB_inline (RuntimeArray* __this, const RuntimeMethod* method) 
{
	{
		RawData_t37CAF2D3F74B7723974ED7CEEE9B297D8FA64ED0* L_0;
		L_0 = il2cpp_unsafe_as<RawData_t37CAF2D3F74B7723974ED7CEEE9B297D8FA64ED0*>(__this);
		NullCheck(L_0);
		uint8_t* L_1 = (uint8_t*)(&L_0->___Data);
		return L_1;
	}
}
// Method Definition Index: 23537
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR void ReadOnlySpan_1__ctor_m0FC0B92549C2968E80B5F75A85F28B96DBFCFD63_gshared_inline (ReadOnlySpan_1_tA850A6C0E88ABBA37646A078ACBC24D6D5FD9B4D* __this, uint8_t* ___0_ptr, int32_t ___1_length, const RuntimeMethod* method) 
{
	{
		uint8_t* L_0 = ___0_ptr;
		ByReference_1_t9C85BCCAAF8C525B6C06B07E922D8D217BE8D6FC L_1;
		memset((&L_1), 0, sizeof(L_1));
		il2cpp_codegen_by_reference_constructor((Il2CppByReference*)(&L_1), L_0);
		__this->____pointer = L_1;
		int32_t L_2 = ___1_length;
		__this->____length = L_2;
		return;
	}
}
